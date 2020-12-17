using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AutomaticPlay : MonoBehaviour
{
    // Start is called before the first frame update
    private bool _preloaded;
    void OnEnable()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "_preload")
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }
}
