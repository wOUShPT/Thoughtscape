using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    void Awake()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }
}
