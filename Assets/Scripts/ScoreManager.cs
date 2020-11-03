using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private float _score;
   
    //Sets score to default (zero)
    void Awake()
    {
        _score = 0;
    }

    //sets the score based on increment or decrement passed through 
    public void Score(float value)
    {
        _score += value;
        
        //Debug info about score values
        Debug.Log("Score:" + _score.ToString());
    }
}
