using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    void Awake()
    {
        LoadGameScene();
    }
    
    //Load main loop scene 
    public static void LoadGameScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }
}
