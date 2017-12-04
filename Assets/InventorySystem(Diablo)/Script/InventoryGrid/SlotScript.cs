using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotScript : MonoBehaviour
{
    public IntVector2 gridPos;
    public Text text;

    public GameObject storedItemObject;
    public IntVector2 storedItemSize;
    public IntVector2 storedItemStartPos;
    public ItemClass storedItemClass;
    public bool isOccupied;

    private GameObject curtain;

    private void Start()
    {
        text.text = gridPos.x + "," + gridPos.y;
        curtain = transform.Find("Curtain").gameObject;
        var curtainParent = GameObject.Find("Canvas/InventoryPanel/MiscParent/CurtainParent").transform;
        curtain.transform.SetParent(curtainParent);
    }

    public void SetActiveCurtain(bool value)
    {
        curtain.SetActive(value);
    }

}
