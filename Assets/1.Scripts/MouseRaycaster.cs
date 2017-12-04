using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseRaycaster : SingletonWithMonoB<MouseRaycaster>
{
    public bool IsHit;

    public RaycastHit RayHit;

    int layerMask;                      // A layer mask so that a ray can be cast just at gameobjects on the floor layer.
    float camRayLength = 100f;          // The length of the ray from the camera into the scene.

    // Use this for initialization
    void Start () {
        IsHit = false;

        // Init raycast layermask
        int floorLayer = LayerMask.GetMask("Floor");
        int itemLayer = LayerMask.GetMask("Item");
        layerMask = floorLayer | itemLayer;
	}
	
    void FixedUpdate()
    {
        // Create a ray from the mouse cursor on screen in the direction of the camera.
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        IsHit = Physics.Raycast(camRay, out RayHit, camRayLength, layerMask);

        //Debug.Log(RayHit.transform.name);
    }

    /*
	// Update is called once per frame
	void Update () {
		
	}*/
}
