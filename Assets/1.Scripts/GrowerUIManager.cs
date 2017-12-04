using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrowerUIManager : MonoBehaviour {

    [HideInInspector]
    public bool IsOpened = false;

    Grower currentGrower;   // TODO : 패널을 닫을 때 이 변수를 null로 바꿔주는 게 좋을 수 있음.

    GameObject growNewPanel; // 새 작물 심을 때 나오는 패널
    GameObject growStatPanel; // 생산중인 작물의 상태 및 수확하는 패널
    GameObject noSeedPanel; // 인벤토리에 씨앗이 없는 경우 나오는 패널
    GameObject harvestButton;

    GameObject[] gauge = new GameObject[5];

    Text cropName;
    Image cropImage;

    Action sowButtonCB;

	// Use this for initialization
	void Start () {
        growNewPanel = transform.GetChild(0).gameObject;
        growStatPanel = transform.GetChild(1).gameObject;
        noSeedPanel = transform.GetChild(2).gameObject;

        harvestButton = growStatPanel.transform.Find("HarvestButton_on").gameObject;
        harvestButton.SetActive(false);

        for(int i = 0; i < gauge.Length; i++)
            gauge[i] = growStatPanel.transform.Find("Gauge/gauge" + i.ToString()).gameObject;   // TODO : USE STRBUILDER 스트링빌더 쓰기

        cropName = growStatPanel.transform.Find("NameText").GetComponent<Text>();
        cropImage = growStatPanel.transform.Find("Frame/plantImage").GetComponent<Image>();

        var xButtonOnNewPanel = growNewPanel.transform.Find("XButton");
        var xButtonOnStatPanel = growStatPanel.transform.Find("XButton");
        var xButtonOnNoSeedPanel = noSeedPanel.transform.Find("XButton");
        xButtonOnNewPanel.GetComponent<Button>().onClick.AddListener(() =>
        {
            growNewPanel.SetActive(false);
            IsOpened = false;

            UIController.Instance.DeactivateBlocker();
        });
        xButtonOnStatPanel.GetComponent<Button>().onClick.AddListener(() =>
        {
            growStatPanel.SetActive(false);
            IsOpened = false;

            UIController.Instance.DeactivateBlocker();
        });
        xButtonOnNoSeedPanel.GetComponent<Button>().onClick.AddListener(() =>
        {
            noSeedPanel.SetActive(false);
            IsOpened = false;

            UIController.Instance.DeactivateBlocker();
        });
    }

    public void OpenGrowNewPanel(Grower grower, Action sowButtonCallback)
    {
        currentGrower = grower;

        growNewPanel.SetActive(true);
        IsOpened = true;

        sowButtonCB = sowButtonCallback;
    }

    public void OpenGrowStatPanel(Grower grower)
    {
        currentGrower = grower;

        SetPanelInfo();

        growStatPanel.SetActive(true);
        IsOpened = true;

        Debug.Log(currentGrower.GrowingCrop);
    }

    public void OpenGrowStatPanel(ItemClass seed)
    {
        currentGrower.SetCrop(seed);

        SetPanelInfo();

        growStatPanel.SetActive(true);
        growNewPanel.SetActive(false);
        IsOpened = true;

        Debug.Log(currentGrower.GrowingCrop);
    }

    public void OpenNoSeedPanel()
    {
        growNewPanel.SetActive(false);

        noSeedPanel.SetActive(true);
        IsOpened = true;
    }

    public void SowButtonClicked()
    {
        //growNewPanel.SetActive(false);
        // TODO : 씨앗 아이템만 하이라이트 하도록 특별한 시퀀스로 열어야 한다.
        sowButtonCB();
    }

    public void HarvestButtonClicked()
    {
        Crop cropToHarvest = currentGrower.GrowingCrop;
        int harvestAmount = currentGrower.Harvest();

        Debug.Log("수확할 작물 갯수 : " + harvestAmount);

        for (int i = 0; i < harvestAmount; i++)
        {
            var itemObj = Instantiate(Resources.Load<GameObject>("ItemObjectPrefabs/" + cropToHarvest)); // TODO : USE STRBUILDER 스트링빌더 쓰기
            var itemClass = itemObj.GetComponent<ItemClass>();      // TODO : 디비로부터 아이템 정보 셋팅하는 부분 좀 더 효율적으로 변경하기
            ItemClass.SetItemValues(itemClass);
            itemObj.SetActive(false);

            Inventory.Instance.TakeItem(itemClass, ignoreDistRestrict : true);
        }

        growStatPanel.SetActive(false);
        growNewPanel.SetActive(true);
    }

    void SetPanelInfo()
    {
        cropName.text = currentGrower.GrowingCrop.ToString();
        cropImage.sprite = Resources.Load<Sprite>("ItemIcons/" + currentGrower.GrowingCrop.ToString());// TODO : USE STRBUILDER 스트링빌더 쓰기

        for (int i = 0; i < gauge.Length; i++)
            gauge[i].SetActive(i == (int)currentGrower.GrowingStep);

        if (currentGrower.GrowingStep == GrowingStep.HarvestTime)
            harvestButton.SetActive(true);
        else
            harvestButton.SetActive(false);
    }
}
