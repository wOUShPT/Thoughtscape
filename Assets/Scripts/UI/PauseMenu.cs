using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using ChromaticAberration = UnityEngine.Rendering.Universal.ChromaticAberration;

public class PauseMenu : MonoBehaviour
{
    //public Volume postProcessingVolume;
    //private ChromaticAberration _chromaticAberration;
    public Sprite pauseSprite;
    public Sprite resumeSprite;
    private Button _pauseButton;
    private Image _imageComponent;
    public bool isPaused;
    void Start()
    {
        isPaused = false;
        //postProcessingVolume.profile.TryGet(out _chromaticAberration);
        _pauseButton = GetComponent<Button>();
        _imageComponent = GetComponent<Image>();
        _pauseButton.onClick.AddListener(Pause);
    }

    void SpriteChange()
    {
        if (isPaused)
        {
            //_chromaticAberration.intensity.value = 0.2f;
            _imageComponent.sprite = resumeSprite;
        }
        else
        {
            //_chromaticAberration.intensity.value = 0;
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
