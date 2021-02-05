using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    private SceneManager _sceneManager;
    private AudioManager _audioManager;
    public OptionsMenu optionsMenu;
    public Animator introAnimator;
    public Animator logoAnimator;
    public Animator swipeArrowAnimator;
    private SwipeDetection _swipeBehaviour;
    void Start()
    {
        _sceneManager = FindObjectOfType<SceneManager>();
        _audioManager = FindObjectOfType<AudioManager>();
        _swipeBehaviour = FindObjectOfType<SwipeDetection>();
        optionsMenu.optionsToggle.onValueChanged.AddListener(ShowHideArrow);
        _audioManager.PlayMainMenuAmbience();
        _swipeBehaviour.swipeEvent.AddListener(Play);
    }

    private void OnDisable()
    {
        _audioManager.StopMainMenuAmbience();
        _swipeBehaviour.swipeEvent.RemoveListener(Play);
        optionsMenu.optionsToggle.onValueChanged.RemoveListener(ShowHideArrow);
    }

    void Play(Vector2 direction)
    {
        if (direction == Vector2.up && !optionsMenu.isToggled)
        {
            swipeArrowAnimator.SetTrigger("Start");
            logoAnimator.SetTrigger("Start");
            introAnimator.SetTrigger("Start");
            optionsMenu.Show(false);
            StartCoroutine(_sceneManager.WaitTimeToLoad(6f, 2));
        }
    }

    void ShowHideArrow(bool state)
    {
        swipeArrowAnimator.gameObject.SetActive(state);
    }
}
