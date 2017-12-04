using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MetroToOutDoor : InteractableObject {

    public override void Interaction()
    {
        SceneManager.LoadScene("2.City_Venas");
    }

    public override void CustomStart()
    {
        
    }
}
