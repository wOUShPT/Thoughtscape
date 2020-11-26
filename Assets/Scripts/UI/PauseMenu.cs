using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public Sprite pauseSprite;
    public Sprite resumeSprite;
    private Button _pauseButton;
    private Image _imageComponent;
    public bool isPaused;
    void Start()
    {
        isPaused = false;
        _pauseButton = GetComponent<Button>();
        _imageComponent = GetComponent<Image>();
        _pauseButton.onClick.AddListener(Pause);
    }

    void SpriteChange()
    {
        if (isPaused)
        {
            _imageComponent.sprite = resumeSprite;
        }
        else
        {
            _imageComponent.sprite = pauseSprite;
        }
    }

    void Pause()
    {
        if (isPaused)
        {
            Time.timeScale = 1;
            isPaused = false;
            SpriteChange();
        }
        else
        {
            Time.timeScale = 0;
            isPaused = true;
            SpriteChange();
        }
    }
}
