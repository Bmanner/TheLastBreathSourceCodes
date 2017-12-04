using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{

    public abstract void Interaction();
    //Template method pattern : To prevent hiding this class's Start() method by derived class
    public abstract void CustomStart();

    [Tooltip("만약 이 스크립트가 적용된 오브젝트와 아웃라인될 오브젝트가 다르다면, 아웃라인 될 오브젝트(Outline 스크립트가 있는 오브젝트)를 이곳에 넣어주세요")]
    public cakeslice.Outline OutlineObject;

    cakeslice.Outline outliner;

    protected void Start()
    {
        outliner = GetComponent<cakeslice.Outline>();

        if(outliner == null)
        {
            outliner = OutlineObject;
        }

        outliner.enabled = false;

        CustomStart();
    }

    void Update()
    {

    }

    void OnMouseDown()
    {
        if (!PlayerMovement.IsCombat)
            Interaction();
    }

    void OnMouseEnter()
    {
        //outliner.enabled = true;
    }

    void OnMouseExit()
    {
        outliner.enabled = false;
    }

    protected void ForceClearOutline()
    {
        outliner.enabled = false;
    }
}

/*LEGACY
public abstract class InteractableObject : MonoBehaviour {

    public abstract void Interaction();
    //Template method pattern : To prevent hiding this class's Start() method by derived class
    public abstract void CustomStart();

    [Tooltip("만약 이 스크립트가 적용된 오브젝트와 아웃라인될 오브젝트가 다르다면, 아웃라인 될 오브젝트를 이곳에 넣어주세요")]
    public GameObject OutlineObject;

    MeshRenderer meshRenderer;
    Material[] originalMat;
    List<Material> outlinedMatList;

    protected void Start()
    {
        if (OutlineObject != null)
            meshRenderer = OutlineObject.GetComponent<MeshRenderer>();
        else
            meshRenderer = transform.GetComponent<MeshRenderer>();

        originalMat = meshRenderer.materials;
        outlinedMatList = new List<Material>();
        outlinedMatList.AddRange(originalMat);
        outlinedMatList.AddResource("Mat_Outline");

        CustomStart();
    }

    void Update()
    {

    }

    void OnMouseDown()
    {
        if(!PlayerMovement.IsCombat)
            Interaction();
    }

    void OnMouseEnter()
    {
        meshRenderer.materials = outlinedMatList.ToArray();
    }

    void OnMouseExit()
    {
        meshRenderer.materials = originalMat;
    }

    protected void ForceClearOutline()
    {
        meshRenderer.materials = originalMat;
    }
}
*/
