using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour
{
    void Awake()
    {
        LoadMainMenu();
    }
    
    //Load Main Menu screen
    public void LoadMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    //Load Main Loop scene 
    public void LoadGameScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(2);
        
        //Delete another existing Game Manager to prevent duplication
        if (GameObject.FindGameObjectWithTag("Managers").GetComponentInChildren<GameManager>())
        {
            Destroy(GameObject.FindGameObjectWithTag("Managers").GetComponentInChildren<GameManager>().gameObject);
        }
    }
}
