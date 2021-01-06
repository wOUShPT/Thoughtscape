using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ScoreBoard : MonoBehaviour
{
    private SceneManager _sceneManager;
    public Score scoreData;
    public TextMeshProUGUI quote;
    public QuotesScriptableObject quotesData;
    public TextMeshProUGUI lastScore;
    public TextMeshProUGUI bestScore;
    public UIFallAnimation textFallAnimation;
    public Button restartButton;
    void Awake()
    {
        _sceneManager = FindObjectOfType<SceneManager>();
        restartButton.onClick.AddListener(RestartGame);
        quote.text = quotesData.QuotesList[Random.Range(0, quotesData.QuotesList.Count)];
        lastScore.text = "You did " + scoreData.lastScore.ToString() + " points";
        bestScore.text = "Your best score is " + scoreData.bestScore.ToString() + " points";
    }

    private void OnDisable()
    {
        restartButton.onClick.RemoveListener(RestartGame);
    }

    void RestartGame()
    {
        textFallAnimation.Animation(0.5f);
        StartCoroutine(_sceneManager.WaitTimeToLoad(0.25f, 2));
    }
}
