using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    private SceneManager _sceneManager;
    public Animator introAnimator;
    public UIFallAnimation playButtonFallAnimation;
    public Button playButton;
    void Awake()
    {
        _sceneManager = FindObjectOfType<SceneManager>();
        playButton.onClick.AddListener(Play);
    }

    private void OnDisable()
    {
        playButton.onClick.RemoveListener(Play);
    }

    void Play()
    {
        playButtonFallAnimation.Animation(0.5f);
        introAnimator.SetTrigger("Start");
        StartCoroutine(_sceneManager.WaitTimeToLoad(5.5f, 2));
    }

    
}
