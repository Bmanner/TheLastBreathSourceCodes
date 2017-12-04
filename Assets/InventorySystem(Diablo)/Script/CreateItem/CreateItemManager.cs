﻿using UnityEngine;
using UnityEngine.UI;

/* 17.10.16(YY.MM.DD) 그리드에 바로 아이템 추가하는 토글을 포함하도록 변경. 
 * 
 * 
 * */

public class CreateItemManager : MonoBehaviour {

    public LoadItemDatabase itemDB;
    public ItemOverlayScript overlayScript;
    public SimpleObjectPool itemEquipPool;
    public ItemListManager listManager;
    public SortAndFilterManager sortManager;
    
    public Dropdown nameDropdown;
    private int selectedID = 0;
    private bool isRandomType = false;

    public Slider lvlSlider;
    public InputField lvlInput;
    public Toggle randomLvlToggle;
    private int selectedLvl = 1;
    private bool isRandomLvl = false;

    public Slider qualitySlider;
    public Toggle randomQualityToggle;
    public Text qualityText;
    public GameObject QualityPanel;
    private int selectedQuality = 0;
    private bool isRandomQuality = false;

    public Button createButton;
    public Button randomButton;
    public Toggle addToListToggle;
    public bool willAddToList = false;
    public Toggle addToGridToggle;
    public bool willAddToGrid = false;

    private ItemClass item = new ItemClass();

    public InvenGridManager invenManager;

    private void Start()
    {
        nameDropdown.AddOptions(itemDB.TypeNameList);//gets ann itemname form database
        lvlInput.text = "1";
        ItemClass.SetItemValues(item, 0, 1, 0);
    }

    public void ButtonEnter()
    {
        ItemClass.SetItemValues(item, selectedID, selectedLvl, selectedQuality);
        overlayScript.UpdateOverlay2(item, !isRandomType, !isRandomLvl, !isRandomQuality);
    }
    public void ButtonExit()
    {
        ItemClass.SetItemValues(item, selectedID, selectedLvl, selectedQuality);
        overlayScript.UpdateOverlay(null);
    }

#region type dropdown
    public void OnNameDropdownChange(int index)
    {
        selectedID = index;
    }
    public void OnRandomTypeToggle(bool isToggled)
    {
        nameDropdown.interactable = !isToggled;
        isRandomType = isToggled;
        if (!isToggled)
        {
            selectedID = nameDropdown.value;
        }
    }
#endregion

#region lvl slider

    public void OnLvlSliderChange(float value)
    {
        selectedLvl = (int)value;
        lvlInput.text = value.ToString();
    }
    public void OnLvlInputChange(string value)
    {
        if (value != "")
        {
            selectedLvl = int.Parse(value);
        }
        else
        {
            selectedLvl = 0;
        }
        lvlSlider.value = selectedLvl;
    }
    public void OnRandomLvlToggleChange(bool isToggled)
    {
        lvlSlider.interactable = !isToggled;
        lvlInput.interactable = !isToggled;
        isRandomLvl = isToggled;
        if (!isToggled)
        {
            selectedLvl = (int)lvlSlider.value;
        }
    }
    #endregion

#region quality slider 
    //could be dropdown
    public void OnQualitySliderChange(float value)
    {
        selectedQuality = (int)value;
        string str = "";
        switch (selectedQuality)
        {
            case 0: str = "Broken"; break;
            case 1: str = "Normal"; break;
            case 2: str = "Magic"; break;
            case 3: str = "Rare"; break;
            default: break;
        }
        qualityText.text = str;
    }
    public void OnRandomQualityToggleChange(bool isToggled)
    {
        qualitySlider.interactable = !isToggled;
        Image image = QualityPanel.GetComponent<Image>();
        image.color = isToggled ? new Color32(200, 200, 200, 128) : new Color32(255, 255, 255, 255);
        isRandomQuality = isToggled;
        if (!isToggled)
        {
            selectedQuality = (int)qualitySlider.value;
        }
    }

    #endregion

#region button events
    public void CreateItem()//for create button
    {
        if (ItemScript.selectedItem == null)
        {
            if (isRandomType) { selectedID = Random.Range(0, 18); }
            if (isRandomLvl) { selectedLvl = Random.Range(1, 101); }
            if (isRandomQuality) { selectedQuality = Random.Range(0, 4); }
            ItemClass newItem = new ItemClass(item);
            ItemClass.SetItemValues(newItem, selectedID, selectedLvl, selectedQuality);
            SpawnOrAddItem(newItem);
        }
    }
    public void RandomItem() //for random button
    {
        if (ItemScript.selectedItem == null)
        {
            ItemClass newItem = new ItemClass();
            ItemClass.SetItemValues(newItem, Random.Range(0, 18), Random.Range(1, 101), Random.Range(0, 4));
            SpawnOrAddItem(newItem);
        }
    }
    private void SpawnOrAddItem(ItemClass passedItem)
    {
        if (willAddToList)// add to list directly
        {
            sortManager.AddItemToList(passedItem);
            Debug.Log("Item added to list");
        }
        else if(willAddToGrid)// add to grid directly
        {
            GameObject itemObj = itemEquipPool.GetObject();
            itemObj.GetComponent<ItemScript>().SetItemObject(passedItem);
            // 그리드로 직접 저장이 실패한 경우 아이템을 다시 풀로 반환
            if(invenManager.StoreItemDirectly(itemObj) == false)
            {
                itemEquipPool.ReturnObject(itemObj);
            }
        }
        else //spawn item on mouse
        {
            GameObject itemObject = itemEquipPool.GetObject();
            itemObject.GetComponent<ItemScript>().SetItemObject(passedItem);
            ItemScript.SetSelectedItem(itemObject);
        }
        
    }
    public void AddToListToggle(bool isToggled)
    {
        willAddToList = isToggled;
        if(willAddToList && willAddToGrid)
        {
            willAddToGrid = false;
            addToGridToggle.isOn = false;
        }
    }
    public void AddToGridToggle(bool isToggled)
    {
        willAddToGrid = isToggled;
        if (willAddToList && willAddToGrid)
        {
            willAddToList = false;
            addToListToggle.isOn = false;
        }
    }
    #endregion
}
