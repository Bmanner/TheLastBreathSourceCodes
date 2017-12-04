using UnityEngine;
using UnityEngine.UI;

public class InvenGridManager : MonoBehaviour {

    public GameObject[,] slotGrid;
    public SlotScript[,] slotScripts;
    public GameObject highlightedSlot;
    public Transform dropParent;
    [HideInInspector]
    public IntVector2 gridSize;

    public ItemListManager listManager;
    public GameObject selectedButton;

    public SimpleObjectPool itemEquipPool;
    public SimpleObjectPool itemButtonPool;

    /// <summary> This is finalOffset rather than totalOffset. Oh no, same with start position. </summary>
    private IntVector2 totalOffset;
    private IntVector2 checkSize, checkStartPos;
    private IntVector2 otherItemPos, otherItemSize; //*3
    /// <summary> 그리드에서 선택되어 마우스 따라다니고 있는 아이의 원래 Position </summary>
    private IntVector2 retItemOriginPos; 

    private int checkState;
    private bool isOverEdge = false;

    public ItemOverlayScript overlayScript;

    private const int NOT_OCCUPIED = 0;
    private const int ONLY_ONE_OCCUPIED = 1;
    private const int DISABLE = 2;
    /* to do list
     * make the ColorChangeLoop on swap items take arrguements fron the other item, not hte private variables *1
     * transfer the CheckArea() and SlotCheck() into inside RefreshColor() *2
     * have *3 be local variables of CheckArea(). SwapItem() uses the variable, may need to rewrite.
     * TODO : 슬롯 스크립트 동적으로 GetComponent 하는 부분을 캐싱하는 것을 고려.
     */
    private void Start()
    {
        ItemButtonScript.invenManager = this;
    }
    // TODO : 우클릭 시에 원래자리로 돌아가도록 인벤그리드매니저에 추가.
    // TODO : 컬러 체인지 루틴은 빼기
    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            // 씨앗 심기 등 원클릭 사용(?) 모드 일 때
            if (UIController.Instance.IsSelectSequence)
            {
                if (highlightedSlot != null && ItemScript.selectedItem == null) //
                {
                    var slotScript = highlightedSlot.GetComponent<SlotScript>(); // TODO : 이걸 나중에 slotscripts[,] 로 가져올 수 있도록 하는 방법을 생각해보자.
                    if (slotScript.isOccupied)
                    {
                        UIController.Instance.SowSeedToGrower(slotScript.storedItemClass);
                        Inventory.Instance.ExhaustItem(slotScript.storedItemClass);
                        itemEquipPool.ReturnObject(slotScript.storedItemObject);

                        // Remove item from slot
                        slotScript.storedItemObject = null;
                        slotScript.storedItemSize = IntVector2.Zero;
                        slotScript.storedItemStartPos = IntVector2.Zero;
                        slotScript.storedItemClass = null;
                        slotScript.isOccupied = false;

                        ClearCurtains();
                    }
                }
            }
            else
            {
                if (highlightedSlot != null && ItemScript.selectedItem != null && !isOverEdge)
                {
                    switch (checkState)
                    {
                        case NOT_OCCUPIED: //store on empty slots
                            StoreItem(ItemScript.selectedItem);
                            ColorChangeLoop(SlotColorHighlights.Blue, ItemScript.selectedItemSize, totalOffset);
                            ItemScript.ResetSelectedItem();
                            //RemoveSelectedButton(); // 리스트에 있던 애를 지워줌
                            break;
                        case ONLY_ONE_OCCUPIED: //swap items
                            ItemScript.SetSelectedItem(SwapItem(ItemScript.selectedItem));
                            SlotSectorScript.sectorScript.PosOffset();
                            ColorChangeLoop(SlotColorHighlights.Gray, otherItemSize, otherItemPos); //*1
                            RefreshColor(true);
                            //RemoveSelectedButton();
                            break;
                            //case DISABLE일 때에는 아무런 일도 일어나지 않는다.
                    }
                }// retrieve items. 회수랄지 그리드에 이미 있는거 클릭했을 때.
                else if (highlightedSlot != null && ItemScript.selectedItem == null && highlightedSlot.GetComponent<SlotScript>().isOccupied == true)
                {
                    ColorChangeLoop(SlotColorHighlights.Gray, highlightedSlot.GetComponent<SlotScript>().storedItemSize, highlightedSlot.GetComponent<SlotScript>().storedItemStartPos);
                    ItemScript.SetSelectedItem(GetItem(highlightedSlot));
                    SlotSectorScript.sectorScript.PosOffset();
                    RefreshColor(true);
                }
            }
        }
        //right click to return item to list if item is from list
        if (Input.GetMouseButtonDown(1) && ItemScript.selectedItem != null) 
        {/*
            totalOffset = retItemOriginPos;
            StoreItem(ItemScript.selectedItem);

            //RefreshColor(false);
            ItemScript.ResetSelectedItem();
            */
            /*
            invenManager.selectedButton.GetComponent<CanvasGroup>().alpha = 1f;
            invenManager.selectedButton = null;
            itemEquipPool.ReturnObject(ItemScript.selectedItem);
            */
        }
    }

    private void CheckArea(IntVector2 itemSize) //*2
    {
        IntVector2 halfOffset;
        IntVector2 overCheck;
        halfOffset.x = (itemSize.x - (itemSize.x % 2 == 0 ? 0 : 1)) / 2;
        halfOffset.y = (itemSize.y - (itemSize.y % 2 == 0 ? 0 : 1)) / 2;
        totalOffset = highlightedSlot.GetComponent<SlotScript>().gridPos - (halfOffset + SlotSectorScript.posOffset);
        checkStartPos = totalOffset; // Yeah. that finally means the startPos.
        checkSize = itemSize;
        overCheck = totalOffset + itemSize;
        isOverEdge = false;
        //checks if item to stores is outside grid
        if (overCheck.x > gridSize.x)
        {
            checkSize.x = gridSize.x - totalOffset.x;
            isOverEdge = true;
        }
        if (totalOffset.x < 0)
        {
            checkSize.x = itemSize.x + totalOffset.x;
            checkStartPos.x = 0;
            isOverEdge = true;
        }
        if (overCheck.y > gridSize.y)
        {
            checkSize.y = gridSize.y - totalOffset.y;
            isOverEdge = true;
        }
        if (totalOffset.y < 0)
        {
            checkSize.y = itemSize.y + totalOffset.y;
            checkStartPos.y = 0;
            isOverEdge = true;
        }
    }
    private int SlotCheck(IntVector2 itemSize)//*2
    {
        GameObject obj = null;
        SlotScript instanceScript;

        // TODO : 정리. 추가된 코드임.
        if (checkStartPos.x + itemSize.x > gridSize.x || checkStartPos.y + itemSize.y > gridSize.y)
            return DISABLE;

        if (!isOverEdge)
        {
            for (int y = 0; y < itemSize.y; y++)
            {
                for (int x = 0; x < itemSize.x; x++)
                {
                    instanceScript = slotGrid[checkStartPos.x + x, checkStartPos.y + y].GetComponent<SlotScript>();
                    if (instanceScript.isOccupied)
                    {
                        if (obj == null)
                        {
                            obj = instanceScript.storedItemObject;
                            otherItemPos = instanceScript.storedItemStartPos;
                            otherItemSize = obj.GetComponent<ItemScript>().item.Size;
                        }
                        else if (obj != instanceScript.storedItemObject)
                            return DISABLE; // if cheack Area has 1+ item occupied
                    }
                }
            }
            if (obj == null)
                return NOT_OCCUPIED; // if checkArea is not accupied
            else
                return ONLY_ONE_OCCUPIED; // if checkArea only has 1 item occupied
        }
        return DISABLE; // check areaArea is over the grid
    }
    /// <summary>
    /// This is called every time when quad(SlotSector in this code) detect OnPointerEnter().
    /// </summary>
    /// <param name="enter"></param>
    public void RefreshColor(bool enter)
    {
        if (enter)
        {
            CheckArea(ItemScript.selectedItemSize);
            checkState = SlotCheck(checkSize);
            switch (checkState)
            {
                case NOT_OCCUPIED: ColorChangeLoop(SlotColorHighlights.Green, checkSize, checkStartPos); break; //no item in area
                case ONLY_ONE_OCCUPIED:
                    ColorChangeLoop(SlotColorHighlights.Yellow, otherItemSize, otherItemPos); //1 item on area and can swap
                    ColorChangeLoop(SlotColorHighlights.Green, checkSize, checkStartPos);
                    break;
                case DISABLE: ColorChangeLoop(SlotColorHighlights.Red, checkSize, checkStartPos); break; //invalid position (more then 2 items in area or area is outside grid)
            }
        }
        else //on pointer exit. resets colors
        {
            isOverEdge = false;
            //checkArea(); //commented out for performance. may cause bugs if not included
            
            ColorChangeLoop2(checkSize, checkStartPos);
            if (checkState == 1)
            {
                ColorChangeLoop(SlotColorHighlights.Blue2, otherItemSize, otherItemPos);
            }
        }
    }
    public void ColorChangeLoop(Color32 color, IntVector2 size, IntVector2 startPos)
    {
        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                slotGrid[startPos.x + x, startPos.y + y].GetComponent<Image>().color = color;
            }
        }
    }
    public void ColorChangeLoop2(IntVector2 size, IntVector2 startPos)
    {
        GameObject slot;
        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                //Debug.Log("Slot[" + (startPos.x + x)+", " + (startPos.y + y) + "]");
                slot = slotGrid[startPos.x + x, startPos.y + y];
                if (slot.GetComponent<SlotScript>().isOccupied)
                {
                    slotGrid[startPos.x + x, startPos.y + y].GetComponent<Image>().color = SlotColorHighlights.Blue2;
                }
                else
                {
                    slotGrid[startPos.x + x, startPos.y + y].GetComponent<Image>().color = SlotColorHighlights.Gray;
                }
            }
        }
    }
    private void StoreItem(GameObject item)
    {
        SlotScript instanceScript;
        IntVector2 itemSizeL = item.GetComponent<ItemScript>().item.Size;
        for (int y = 0; y < itemSizeL.y; y++)
        {
            for (int x = 0; x < itemSizeL.x; x++)
            {
                //set each slot parameters
                instanceScript = slotGrid[totalOffset.x + x, totalOffset.y + y].GetComponent<SlotScript>();
                instanceScript.storedItemObject = item;
                instanceScript.storedItemClass = item.GetComponent<ItemScript>().item;
                instanceScript.storedItemSize = itemSizeL;
                instanceScript.storedItemStartPos = totalOffset;
                instanceScript.isOccupied = true;
                slotGrid[totalOffset.x + x, totalOffset.y + y].GetComponent<Image>().color = SlotColorHighlights.Gray;
            }
        }//set dropped parameters
        item.transform.SetParent(dropParent);
        item.GetComponent<RectTransform>().pivot = Vector2.zero;
        item.transform.position = slotGrid[totalOffset.x, totalOffset.y].transform.position;
        item.GetComponent<CanvasGroup>().alpha = 1f;
        //overlayScript.UpdateOverlay(highlightedSlot.GetComponent<SlotScript>().storedItemClass);
    }
    private GameObject GetItem(GameObject slotObject)
    {
        SlotScript slotObjectScript = slotObject.GetComponent<SlotScript>();
        GameObject retItem = slotObjectScript.storedItemObject; // maybe retreived item.
        retItemOriginPos = slotObjectScript.storedItemStartPos;
        IntVector2 itemSizeL = retItem.GetComponent<ItemScript>().item.Size;

        SlotScript instanceScript;
        for (int y = 0; y < itemSizeL.y; y++)
        {
            for (int x = 0; x < itemSizeL.x; x++)
            {
                //reset each slot parameters
                instanceScript = slotGrid[retItemOriginPos.x + x, retItemOriginPos.y + y].GetComponent<SlotScript>();
                instanceScript.storedItemObject = null;
                instanceScript.storedItemSize = IntVector2.Zero;
                instanceScript.storedItemStartPos = IntVector2.Zero;
                instanceScript.storedItemClass = null;
                instanceScript.isOccupied = false;
            }
        }//returned item set item parameters
        retItem.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
        retItem.GetComponent<CanvasGroup>().alpha = 0.5f;
        retItem.transform.position = Input.mousePosition;
        overlayScript.UpdateOverlay(null);
        return retItem;
    }
    private GameObject SwapItem(GameObject item)
    {
        GameObject tempItem;
        tempItem = GetItem(slotGrid[otherItemPos.x, otherItemPos.y]);
        StoreItem(item);
        return tempItem;
    }

    public void RemoveSelectedButton()
    {
        /* comment by Jin: 항상 리스트로부터 아이템이 오니까 리스트에서 삭제해준다는 내용.
        if (selectedButton != null)
        {
            listManager.RevomeItemFromList(selectedButton.GetComponent<ItemButtonScript>().item);
            listManager.RemoveButton(selectedButton);
            listManager.sortManager.SortAndFilterList();
            selectedButton = null;
        }
        */

        // selectedButton = null;
        itemButtonPool.ReturnObject(ItemScript.selectedItem);
    }

    private int CheckEmptySlot()
    {
        SlotScript instanceScript;

        foreach (var slot in slotGrid)
        {
            instanceScript = slot.GetComponent<SlotScript>();
            
        }

        return 0;
    }
    /// <summary>
    /// 그리드로 아이템을 즉시 저장 후, 저장 성공 여부를 반환한다.
    /// </summary>
    /// <param name="item"></param>
    /// <returns> 저장 성공 여부 </returns>
    public bool StoreItemDirectly(GameObject item)
    {
        ItemClass itemClass = item.GetComponent<ItemClass>();   // TODO : GetComp빼기

        GameObject itemObj = itemEquipPool.GetObject();
        ItemScript itemScript = itemObj.GetComponent<ItemScript>();
        itemScript.SetItemObject(itemClass);

        SlotScript instanceScript;
        IntVector2 itemSize = itemScript.item.Size;

        for(int y = 0; y < gridSize.y; y++)
        {
            for(int x = 0; x < gridSize.x; x++)
            {
                instanceScript = slotGrid[x, y].GetComponent<SlotScript>();
                //TODO : 아이템 사이즈 체크해서 1이면 아래 일부 루틴 건너뛰기
                if (instanceScript.isOccupied == false)
                {
                    checkStartPos = new IntVector2(x, y);
                    bool storable = SlotCheck(itemSize) == 0 ? true : false;

                    if(storable)
                    {
                        totalOffset = checkStartPos;
                        StoreItem(itemObj);
                        RefreshColor(false);
                        return storable;
                    }
                }
            }
        }

        // TODO : 공간이 부족해서 넣지 못했다는 메세지 추가하기
        itemEquipPool.ReturnObject(itemObj);

        return false; // 저장에 실패한 경우(공간 부족)
    }

    public void PlaceCurtainIfNotSeed()
    {
        foreach(var item in slotScripts)
        {
            if(item.storedItemClass != null && item.storedItemClass.CategoryID != (int)CategoryID.Seed)
            {
                item.SetActiveCurtain(true);
            }
        }
    }

    public void ClearCurtains()
    {
        foreach (var item in slotScripts)
        {
            item.SetActiveCurtain(false);
        }
    }
}

public struct SlotColorHighlights
{
    public static Color32 Green
    { get { return new Color32(127, 223, 127, 255); } }
    public static Color32 Yellow
    { get { return new Color32(223, 223, 63, 255); } }
    public static Color32 Red
    { get { return new Color32(223, 127, 127, 255); } }
    public static Color32 Blue
    { get { return new Color32(159, 159, 223, 255); } }
    public static Color32 Blue2
    { get { return new Color32(191, 191, 223, 255); } }
    public static Color32 Gray
    { get { return new Color32(223, 223, 223, 255); } }
}
