using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEMP_JerryCon : InteractableObject {

    Transform fatherTF;

    public override void Interaction()
    {
        StartCoroutine(LookAtFather());
    }

    public override void CustomStart()
    {
        fatherTF = GameObject.FindWithTag("Player").transform;
    }
    
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator LookAtFather()
    {
        Quaternion lookAt = Quaternion.LookRotation(fatherTF.position - transform.position);
        Quaternion rot;
        float counter = 0f;
        while (counter <= 5f)
        {
            rot = Quaternion.Lerp(transform.rotation, lookAt, Time.deltaTime*5f);
            transform.rotation = rot;
            counter += Time.deltaTime;
            yield return null;
        }
        yield return null;
    }

}
