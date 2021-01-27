using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DDOL : MonoBehaviour
{
    //Dont Destroy On Load Script (attach to the gameobject)
    void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
