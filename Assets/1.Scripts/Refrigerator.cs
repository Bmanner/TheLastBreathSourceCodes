using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Refrigerator : InteractableObject {
    public Animation Anim;

    Transform playerTrans;

    public override void Interaction()
    {
        //Calculate distance between player and door
        if (Vector3.Distance(playerTrans.position, this.transform.position) < StandardVariables.InteractableDistance)
        {
            if (!Anim.isPlaying)
            {
                Anim.Play("DoorOpen");
                // TODO: 냉장고 UI를 불러온다.
            }
        }
    }

    public override void CustomStart()
    {
        playerTrans = GameObject.Find("Player").transform;
    }

}
