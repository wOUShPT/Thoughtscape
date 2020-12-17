using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    private SceneManager _sceneManager;
    public Button playButton;
    public Rigidbody2D playButtonRb;
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
        StartCoroutine(FallAnimation());
    }
    
    IEnumerator FallAnimation()
    {
        playButtonRb.velocity = Vector2.down * 8;
        yield return new WaitForSeconds(0.5f);
        _sceneManager.LoadScene(2);
    }
}
