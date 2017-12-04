using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorMovement: InteractableObject {
    Animation anim;

    Transform playerTrans;

    public bool doorOpened = false; //false is close, true is open

    public override void Interaction()
    {
        //Calculate distance between player and door
        if (Vector3.Distance(playerTrans.position, this.transform.position) < StandardVariables.InteractableDistance)
        {
            if (!doorOpened && !anim.isPlaying)
            {
                anim.Play("DoorOpen");
                doorOpened = true;
                StartCoroutine(this.CheckPlayerDistance());
            }
        }
    }

    public override void CustomStart()
    {
        anim = GetComponent<Animation>();

        doorOpened = false;
        playerTrans = GameObject.Find("Player").transform;
    }

    void Update()
    {
        //If press F key on keyboard
        if (Input.GetKeyDown(Shortcuts.Interaction)) // TODO : click했을 때로 바꾸어야 함.
        {
            Interaction();
        }
    }

    public IEnumerator CheckPlayerDistance()
    {
        while (Vector3.Distance(playerTrans.position, this.transform.position) < StandardVariables.InteractableDistance || anim.isPlaying)
        {
            yield return null;
        }
        //Change door status
        doorOpened = false;
        anim.Play("DoorClose");
        yield return null;
    }

}
