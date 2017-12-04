using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Obsolete]
public class Item : InteractableObject {

    public int Weight = 1;

    public override void Interaction()
    {
        //Destroy(transform.gameObject); // TODO : Deactivate and activate later when item throw away

        //Test code
        //if success to take this item
        /*
        if (Inventory.Instance.TakeItem(this))
        {
            base.ForceClearOutline();
            gameObject.SetActive(false);
        }
        else
        {

        }
        */
    }

    public override void CustomStart()
    {
        // Nothing to do
    }
	
	void Update () {
		
	}

    void OnMouseDown()
    {
        Interaction();
    }
}
