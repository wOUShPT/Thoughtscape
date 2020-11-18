using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    void Awake()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }
    
    //Load main loop scene 
    public static void LoadGameScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
        if (GameObject.FindGameObjectWithTag("Managers"))
        {
            Destroy(GameObject.FindGameObjectWithTag("Managers").GetComponentInChildren<GameManager>().gameObject);
            Destroy(GameObject.FindGameObjectWithTag("Managers").GetComponentInChildren<SpawnManager>().gameObject);
        }
    }
}
