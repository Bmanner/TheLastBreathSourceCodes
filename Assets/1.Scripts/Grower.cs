using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grower : InteractableObject, ITimeUpdateListener {

    public GameObject[] CropObjParent;

    List<Transform> cropPlaces = new List<Transform>();
    List<GameObject> cropPrefabList = new List<GameObject>();
    List<MeshRenderer> soilRendererList = new List<MeshRenderer>();

    Material originalSoilMat;
    Material sowedSoilMat;

    public Crop GrowingCrop { get { return growingCrop; } }
    Crop growingCrop;
    public GrowingStep GrowingStep { get { return growingStep; } }
    GrowingStep growingStep;

    //TODO : 기획에 따라 추후 수정.
    readonly int daysToGrow = 1;
    int dayPassed = 0;
     
    int harvestAmount;
    int minYield;
    int maxYield;
    int yieldAdder;

    public override void Interaction()
    {
        UIController.Instance.GrowPanelOpen(growingCrop != Crop.None, this);
    }

    public override void CustomStart()
    {
        foreach(GameObject parent in CropObjParent)
        {
            cropPlaces.AddRange(parent.GetComponentsInChildren<Transform>());
            cropPlaces.Remove(parent.transform);
        }
        Debug.Log(cropPlaces.Count);

        soilRendererList.Add(transform.GetChild(7).GetComponent<MeshRenderer>());   // TODO : 확장성을 위해 getchild 형식 바꾸기.
        soilRendererList.Add(transform.GetChild(8).GetComponent<MeshRenderer>());
        soilRendererList.Add(transform.GetChild(10).GetComponent<MeshRenderer>());

        originalSoilMat = soilRendererList[0].material;
        sowedSoilMat = Resources.Load<Material>("Mat_SowedSoil");

        growingCrop = Crop.None;
        growingStep = GrowingStep.Seed;
    }

    void ITimeUpdateListener.TimeUpdate()
    {
        // 작물이 없거나 수확해야 하는 경우 업데이트 콜 무시
        if (growingCrop == Crop.None || growingStep == GrowingStep.HarvestTime)
            return;

        dayPassed++;

        if (daysToGrow <= dayPassed)
        {
            growingStep++;
            dayPassed = 0;

            UpdateModel();

            if (growingStep == GrowingStep.HarvestTime)
                CalcYield();
        }
    }

    public void SetCrop(ItemClass crop)
    {
        growingCrop = (Crop)crop.TypeID;
        minYield = crop.MinYield;
        maxYield = crop.MaxYield;
        yieldAdder = crop.YieldAdder;

        foreach (var soil in soilRendererList)
        {
            soil.material = sowedSoilMat;
        }

        for (int i = 0; i < cropPlaces.Count; i++)
        {
            cropPrefabList.Add(GameObject.Instantiate(Resources.Load<GameObject>("GrowingCropPrefabs/" + growingCrop.ToString()),
                Vector3.zero, Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0))); // TODO : 스트링빌더 쓰기
            cropPrefabList[i].transform.SetParent(cropPlaces[i], true);
            cropPrefabList[i].transform.position = cropPlaces[i].position;
        }
    }

    public int Harvest()
    {
        int returnAmount = harvestAmount;
        harvestAmount = 0;

        growingCrop = Crop.None;
        growingStep = GrowingStep.Seed;
        dayPassed = 0;

        harvestAmount = 0;
        minYield = 0;
        maxYield = 0;
        yieldAdder = 0;

        ResetModel();

        return returnAmount;
    }

    void UpdateModel()
    {
        foreach(var item in cropPrefabList)
        {
            for(int i = 0; i < item.transform.childCount; i++)
            {
                item.transform.GetChild(i).gameObject.SetActive((i+1) == (int)growingStep);
            }
        }
    }

    void ResetModel()
    {
        foreach (var item in cropPrefabList)
            Destroy(item);

        cropPrefabList.Clear();

        foreach (var soilRenderer in soilRendererList)
            soilRenderer.material = originalSoilMat;
    }

    void CalcYield()
    {
        harvestAmount = UnityEngine.Random.Range(minYield, maxYield) + yieldAdder;
    }


}
