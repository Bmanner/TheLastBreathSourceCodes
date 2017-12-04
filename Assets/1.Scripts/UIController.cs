using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class UIController : SingletonWithMonoB<UIController> {

    public Animation InventoryAnim;
    public Animation EquipmentAnim;

    public GrowerUIManager GrowUIManager;
    [HideInInspector] public bool IsSelectSequence;

    GameObject touchBlocker;
    GameObject aeroGlassCultivation;
    GameObject aeroGlassFull;

    Transform inventoryPanel;
    Transform equipmentPanel;
    Transform pausePanel;

    Text dayNumText;
    Text timeNumText;
    Text AMPMText;
    Text dustnumText;

    bool isInvenOpen;
    bool isEquipOpen;
    
	// Use this for initialization
	void Start () {
        Time.timeScale = 1;

        Transform mainCam = Camera.main.transform;

        touchBlocker = mainCam.Find("TouchBlocker").gameObject;
        touchBlocker.SetActive(false);
        aeroGlassCultivation = mainCam.Find("AeroGlassCultivation").gameObject;
        aeroGlassCultivation.SetActive(false);
        aeroGlassFull = mainCam.Find("AeroGlassFull").gameObject;
        aeroGlassFull.SetActive(false);

        inventoryPanel = transform.Find("InventoryPanel");
        equipmentPanel = transform.Find("EquipmentPanel");
        
        pausePanel = transform.Find("PausePanel");
        pausePanel.gameObject.SetActive(false);

        dayNumText = transform.Find("MainPanel/DayInfo/panel_dayInfo/DayPanel/DayNumText").GetComponent<Text>();
        timeNumText = transform.Find("MainPanel/DayInfo/panel_dayInfo/TimePanel/TimeNumText").GetComponent<Text>();
        AMPMText = transform.Find("MainPanel/DayInfo/panel_dayInfo/TimePanel/TimeAMPMText").GetComponent<Text>();
        dustnumText = transform.Find("MainPanel/DayInfo/panel_dayInfo/FineDustPanel/DustNumText").GetComponent<Text>();

        dayNumText.text = TimeManager.GameDay.ToString();
        timeNumText.text = TimeManager.GameHour.ToString();
        AMPMText.text = TimeManager.IsAM ? TimeManager.AM : TimeManager.PM;
        // TODO : dustnumText = 

        IsSelectSequence = false;
        isInvenOpen = false;
        isEquipOpen = false;
	}
	
	void Update () {

        // TODO : 스트링 가비지 체크
        // 시간,날짜 변경 체크
        if(TimeManager.HasUpdate)
        {
            timeNumText.text = TimeManager.GameHour.ToString();

            if (TimeManager.HasAMPMUpdate)
            {
                if (TimeManager.IsAM)
                    AMPMText.text = TimeManager.AM;
                else
                    AMPMText.text = TimeManager.PM;
            }

            if (TimeManager.HasDayUpdate)
                dayNumText.text = TimeManager.GameDay.ToString();
                
        }
        // 인벤토리창 토글
        if (Input.GetKeyDown(Shortcuts.Inventory) && !InventoryAnim.isPlaying)
        {
            InventoryOpenClose();
        }
        // 장비창 토글
        if(Input.GetKeyDown(Shortcuts.Equipment) && !EquipmentAnim.isPlaying)
        {
            EquipOpenClose();
        }
        // PausePanel 토글
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePausePanel();
        }

    }

    public void GrowPanelOpen(bool hasCrop, Grower grower)
    {
        if(hasCrop)
        {
            GrowUIManager.OpenGrowStatPanel(grower);
        }
        else
        {
            GrowUIManager.OpenGrowNewPanel(grower, sowButtonCallback: OpenSeedInven);
        }

        if (!touchBlocker.activeSelf) touchBlocker.SetActive(true);
    }

    public void SowSeedToGrower(ItemClass seedToSow)
    {
        IsSelectSequence = false; // TODO : 셀렉트 시퀀스에 맞추어 ESC키로 인벤토리랑 그로우 패널 한번에 닫는 기능 추가하기.
        InventoryClose();
        GrowUIManager.OpenGrowStatPanel(seedToSow);
    }

    public void DeactivateBlocker()
    {
        touchBlocker.SetActive(GrowUIManager.IsOpened);
    }

    public void ChangeDustText(int value, Color textColor)
    {
        dustnumText.text = value.ToString();
        dustnumText.color = textColor;
    }

    void InventoryOpenClose()
    {
        if (!isInvenOpen)
            InventoryOpen();
        else
            InventoryClose();
    }

    void InventoryOpen()
    {
        if (isInvenOpen)
            return;

        inventoryPanel.gameObject.SetActive(true);
        InventoryAnim.Play("InventoryOpen");
        isInvenOpen = true;
    }

    void InventoryClose()
    {
        if (!isInvenOpen)
            return;

        InventoryAnim.Play("InventoryClose");
        isInvenOpen = false;
        // 인벤토리 창이 완전히 숨겨진 뒤에 디액티브
        StartCoroutine(WaitforAnimation(InventoryAnim, () =>
        {
            if (!isInvenOpen)
                inventoryPanel.gameObject.SetActive(false);
        }));
    }

    void EquipOpenClose()
    {
        if (!isEquipOpen)
        {
            // 장비창 오픈
            equipmentPanel.gameObject.SetActive(true);
            EquipmentAnim.Play("EquipmentOpen");
            isEquipOpen = true;
        }
        else
        {
            // 장비창 클로즈
            EquipmentAnim.Play("EquipmentClose");
            isEquipOpen = false;
            // 장비창이 완전히 숨겨진 뒤 디액티브
            StartCoroutine(WaitforAnimation(EquipmentAnim, () =>
            {
                if (!isEquipOpen)
                    equipmentPanel.gameObject.SetActive(false);
            }));
        }
    }

    void TogglePausePanel()
    {
        if (pausePanel.gameObject.activeSelf == true)
        {
            Time.timeScale = 1;

            pausePanel.gameObject.SetActive(false);
            aeroGlassFull.gameObject.SetActive(false);
        }
        else
        {
            Time.timeScale = 0;

            pausePanel.gameObject.SetActive(true);
            aeroGlassFull.gameObject.SetActive(true);
        }
    }

    public void ResumeClicked()
    {
        TogglePausePanel();
    }

    public void MainMenuClicked()
    {
        SceneManager.LoadScene("0.TitleScene");
    }

    public void ExitClicked()
    {
        Debug.Log("종료");
        Application.Quit();
    }

    void OpenSeedInven()
    {
        // 씨앗이 인벤토리에 있는지 체크
        if(Inventory.Instance.HaveSeed())
        {
            InventoryOpen();
            Inventory.Instance.StartSeedSelectSeq();
        }
        else
        {
            GrowUIManager.OpenNoSeedPanel();
        }
    }

    IEnumerator WaitforAnimation(Animation anim, Action callback)
    {
        while(anim.isPlaying)
        {
            yield return null;
        }

        callback();
    }
    
}
