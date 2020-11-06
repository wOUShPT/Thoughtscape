using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public Text remainingTimeText;
    public Text scoreText;
    public float remainingTimeSeconds;

    private float _remainingTimeMinutes;
    private float _timerSeconds;
    private float _score;
    
    void Awake()
    {
        //Set timer to default (zero)
        _timerSeconds = 0;
        _remainingTimeMinutes = 0;
        //Sets score to default (zero)
        _score = 0;
        scoreText.text = 0.ToString();
        
        //Sets the "Managers" gameobject on hierarchy as a parent (this matters if you load the game from _preload scene)
        if (GameObject.FindGameObjectWithTag("Managers"))
        {
            transform.SetParent(GameObject.FindGameObjectWithTag("Managers").transform);
        }
    }

    void Update()
    {
        _timerSeconds += Time.deltaTime;
        remainingTimeSeconds -= Time.deltaTime;
        _remainingTimeMinutes = TimeSpan.FromSeconds(remainingTimeSeconds).Minutes;
        remainingTimeSeconds = Mathf.Clamp(remainingTimeSeconds - Time.deltaTime, 0, Mathf.Infinity);
        remainingTimeText.text = $"{_remainingTimeMinutes}:{remainingTimeSeconds}";
    }

    //sets the score based on increment or decrement passed through 
    public void Score(float value)
    {
        _score += value;
        scoreText.text = _score.ToString();
        //Debug info about score values
        Debug.Log("Score:" + _score.ToString());
    }
}
