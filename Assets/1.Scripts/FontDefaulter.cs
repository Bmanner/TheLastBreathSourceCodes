// The PrintAwake script is placed on a GameObject.  The Awake function is
// called when the GameObject is started at runtime.  The script is also
// called by the Editor.  An example is when the scene is changed to a
// different scene in the Project window.
// The Update() function is called, for example, when the GameObject transform
// position is changed in the Editor.

// ####################################################################
// FontDefaulter by Jinwon Kim.

// This script is for new created "Text UI". 
// This script sets the default font of the text to "이순신 돋음체L" an
// the color of it to white when a new text is created.
// 는 쓸데없는 짓이고 업데이트가 너무 항상 불려서... 폐기
// ####################################################################

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[ExecuteInEditMode]
public class FontDefaulter : MonoBehaviour {
    /*
    List<Text> childTexts;

	// Use this for initialization
	void Awake () {
        childTexts = new List<Text>();
        childTexts.AddRange(transform.GetComponentsInChildren<Text>());
        Debug.Log("Count of text components in Canvas : " + childTexts.Count);
	}
	
	// Update is called once per frame
	void Update () {
        Debug.Log("Font Defaulter update called.");
    }*/
}