using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using ChromaticAberration = UnityEngine.Rendering.Universal.ChromaticAberration;

public class OptionsMenu : MonoBehaviour
{
    private Animator _animator;
    private AudioManager _audioManager;
    private InputManager _inputManager;
    public Toggle vibrationToggle;
    public Toggle optionsToggle;
    private Image _optionsToggleImage;
    public Toggle muteToggle;
    public Toggle creditsToggle;
    public ScoreScriptableObject scoreScriptableObjectScriptableObject;
    public TextMeshProUGUI scoreText;
    public Image muteImageComponent;
    public Image vibrationImageComponent;
    public bool isToggled;
    void Awake()
    {
        isToggled = false;
        _animator = GetComponent<Animator>();
        _audioManager = FindObjectOfType<AudioManager>();
        _inputManager = FindObjectOfType<InputManager>();
        _optionsToggleImage = optionsToggle.GetComponent<Image>();
        optionsToggle.onValueChanged.AddListener(Pause);
        muteToggle.onValueChanged.AddListener(_audioManager.Mute);
        vibrationToggle.onValueChanged.AddListener(_inputManager.ToggleVibrate);
        muteToggle.isOn = _audioManager.isMuted;
        vibrationToggle.isOn = _inputManager.canVibrate;
        scoreText.text = "Your best score is " + scoreScriptableObjectScriptableObject.bestScore;
    }

    void Pause(bool state)
    {
        if (state)
        {
            isToggled = false;
            _animator.SetBool("isPaused", false);
        }
        else
        {
            isToggled = true;
            _animator.SetBool("isPaused", true);
        }
    }

    private void OnDisable()
    {
        optionsToggle.onValueChanged.RemoveListener(Pause);
        muteToggle.onValueChanged.RemoveListener(_audioManager.Mute);
        vibrationToggle.onValueChanged.RemoveListener(_inputManager.ToggleVibrate);
    }

    void SetTimeScale()
    {
        if (isToggled)
        {
            Time.timeScale = 0;
            _audioManager.showerLoop.volume = 0;
            _audioManager.cityLoop.volume = 0;
        }
        else
        {
            Time.timeScale = 1;
            _audioManager.showerLoop.volume = 1;
            _audioManager.cityLoop.volume = 1;
        }
    }

    void KeepToggled(Toggle toggle, bool state)
    {
        toggle.isOn = toggle.isOn;
    }

    public void Hide()
    {
        _optionsToggleImage.color = new Color(_optionsToggleImage.color.r, _optionsToggleImage.color.g, _optionsToggleImage.color.b, 0);
        optionsToggle.interactable = false;
    }

    public void Show()
    {
        _optionsToggleImage.color = new Color(_optionsToggleImage.color.r, _optionsToggleImage.color.g, _optionsToggleImage.color.b, 1);
        optionsToggle.interactable = true;
    }
}
