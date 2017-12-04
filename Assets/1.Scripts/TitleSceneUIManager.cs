using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class TitleSceneUIManager : MonoBehaviour {

    void Start()
    {
        Time.timeScale = 1;
    }

    public void PlayClicked()
    {
        SceneManager.LoadScene("1.MetroScene");
    }

    public void ExitClicked()
    {
        Debug.Log("종료");
        Application.Quit();
    }

}
