using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO : 후에 셰이더를 직접 작성하는 것도 하나의 방법일 듯 합니다. 
// 렌더러의 머티리얼 사이즈를 늘리고 줄이고 하는 게 성능에 얼마나 영향을 미치는지 모르겠음.

public class GameUIManager : MonoBehaviour {

    public Texture2D cursorTexture;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;

    /*
    void OnMouseEnter()
    {
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
    }

    void OnMouseExit()
    {
        Cursor.SetCursor(null, Vector2.zero, cursorMode);
    }
    */



    
}



/*  ## OnMouseOver 같은거 잠깐 모르고 썼던 코드...멍청했다... ##
    public Material OutlineMaterial;

    Transform item;
    Material[] matTemp; // 아이템의 원래 머티리얼을 임시적으로 들고 있을 변수

    List<Material> matList;

    int itemLayer;    

    void Start() {
        matList = new List<Material>();
        matList.Add(OutlineMaterial);

        itemLayer = LayerMask.NameToLayer("Item");
    }
    
    void Update() {

    }

    void FixedUpdate()
    {
        //성능 테스트 내용
        //var test = MouseRaycaster.Instance.RayHit.transform;
        //for(int i = 0; i < 10; i++)
        //{
        //  if (i == 100) // for 1000000에 최대 14ms정도 나옴 AVG는 7ms정도
        //  i++;
        //  if (MouseRaycaster.Instance.RayHit.transform == null && i == 100) // for 10000번에 최대 8 평균 4ms정도
        //      i++;
        //  if (test == null && i == 100) // for 10000번에 0.6ms 정도로 훅 줄어듬.
        //      i++;
        //}
        

        // 아이템 위에 마우스를 올리면 아이템에 외곽선을 그려준다.
        if (MouseRaycaster.Instance.IsHit)
        {
            if (MouseRaycaster.Instance.RayHit.transform.gameObject.layer == itemLayer)
            {
                if (item == null)
                {
                    item = MouseRaycaster.Instance.RayHit.transform;

                    matTemp = item.GetComponent<MeshRenderer>().materials; // TODO : 최적화 필요할수도 있음

                    matList.AddRange(matTemp);
                    item.GetComponent<MeshRenderer>().materials = matList.ToArray();
                }                
                else if (MouseRaycaster.Instance.RayHit.transform != item)
                {
                    // 기존 아이템의 외곽선을 해제
                    item.GetComponent<MeshRenderer>().materials = matTemp;
                    matList.RemoveRange(1, matList.Count - 1);

                    // 새 아이템의 외곽선을 그려준다
                    item = MouseRaycaster.Instance.RayHit.transform;

                    matTemp = item.GetComponent<MeshRenderer>().materials; // TODO : 최적화 필요할수도 있음

                    matList.AddRange(matTemp);
                    item.GetComponent<MeshRenderer>().materials = matList.ToArray();
                }
            }
            else
            {
                // 외곽선 해제 작업
                if (item != null)
                {
                    item.GetComponent<MeshRenderer>().materials = matTemp;

                    item = null;
                    matTemp = null;
                    matList.RemoveRange(1, matList.Count - 1);
                }
            }
        }
        //else
        //{
            // 외곽선 해제 작업
            //if (item != null)
            //{
                //item.GetComponent<MeshRenderer>().materials = matTemp;
                //
                //item = null;
                //matTemp = null;
                //matList.RemoveRange(1, matList.Count - 1);
            //}
        //}
    }
*/
