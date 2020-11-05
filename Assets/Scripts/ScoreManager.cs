using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private float _score;
    
    void Awake()
    {
        //Sets score to default (zero)
        _score = 0;
        
        //Sets the "Managers" gameobject on hierarchy as a parent (this matters if you load the game from _preload scene)
        if (GameObject.FindGameObjectWithTag("Managers"))
        {
            transform.SetParent(GameObject.FindGameObjectWithTag("Managers").transform);
        }
    }

    //sets the score based on increment or decrement passed through 
    public void Score(float value)
    {
        _score += value;
        
        //Debug info about score values
        Debug.Log("Score:" + _score.ToString());
    }
}
