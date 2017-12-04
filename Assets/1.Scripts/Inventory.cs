using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/* TODO List
 * 아이템창 꽉 찬 상태에서 수확할 때 문제점 수정.
 * 
 * 
 * 
 */

public class Inventory : SingletonWithMonoB<Inventory> {

#if UNITY_EDITOR
    [ReadOnly]
#endif
    public int CurrentOccupiedCapacity;

    public GameObject InventoryPanel;
    public Transform PlayerTF;

    [HideInInspector] public float SlotSize;

    InvenGridManager gridManager;
    List<ItemClass> havingItemList;

    int MaxCapacity;

    void Start () {
        gridManager = InventoryPanel.GetComponentInChildren<InvenGridManager>();
        havingItemList = new List<ItemClass>();

        MaxCapacity = gridManager.gridSize.x * gridManager.gridSize.y;
        //slotSize = GameObject.FindGameObjectWithTag("InvenPanel").GetComponent<InvenGridScript>().slotSize;
        SlotSize = gridManager.GetComponent<InvenGridScript>().slotSize;

        StartCoroutine(LateStart());
    }

    IEnumerator LateStart()
    {
        yield return null;

        InventoryPanel.SetActive(false);
    }

	void Update () {
		
	}

    public bool TakeItem(ItemClass itemToPut, bool ignoreDistRestrict = false)
    {
        if(!ignoreDistRestrict && Vector3.Distance(itemToPut.transform.position, transform.position) > StandardVariables.InteractableDistance)
        {
            return false;
        }

        if (CurrentOccupiedCapacity < MaxCapacity && gridManager.StoreItemDirectly(itemToPut.gameObject))
        {
            CurrentOccupiedCapacity += itemToPut.Weight;

            havingItemList.Add(itemToPut);

            // TODO : 아이템 기록 로그
            Debug.Log("아이템을 입수했습니다. Inventory : (" + MaxCapacity + "/" + CurrentOccupiedCapacity + "). 입수 아이템 : " + itemToPut.name);
            
            return true; // Item put success
        }
        else
        {
            Debug.Log("가방이 꽉 차서 아이템을 넣을 수 없습니다.");

            return false; // Item put fail
        }
    }

    public void ThrowItem(GameObject throwItem)
    {
        // If there is over an Item
        if(havingItemList.Count > 0)
        {
            var itemToThrow = havingItemList.First(obj => obj == throwItem.GetComponent<ItemScript>().item); // TODO : getComponent 수정
            if (itemToThrow != null)
            {
                CurrentOccupiedCapacity -= itemToThrow.Weight;

                havingItemList.Remove(itemToThrow);

                //Test code
                itemToThrow.transform.position = new Vector3(transform.position.x, itemToThrow.transform.position.y, transform.position.z) + transform.forward * 0.2f;
                //itemToThrow.transform.rotation = transform.rotation;
                itemToThrow.gameObject.SetActive(true);

                gridManager.RemoveSelectedButton();
                ItemScript.ResetSelectedItem();

                // TODO : 버리는 아이템들이 겹치지 않도록 체크.
            }
        }
        else
        {
            Debug.Log("가방에 아이템이 아무것도 없습니다. 버릴게 없음. ㅎ");
        }
    }

    public void ExhaustItem(ItemClass item)
    {
        if (havingItemList.Count > 0)
        {
            havingItemList.Remove(item);

            Debug.Log(havingItemList.Count);
        }
    }

    public void StartSeedSelectSeq()
    {
        UIController.Instance.IsSelectSequence = true;
        gridManager.PlaceCurtainIfNotSeed();
    }

    public bool HaveSeed()
    {
        foreach(var item in havingItemList)
        {
            if (item.CategoryID == (int)CategoryID.Seed)
                return true;
        }

        return false;
    }
}
