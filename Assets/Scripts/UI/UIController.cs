using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    [Space(30, order = 0)]
    [Header("UI Settings", order = 1)]
    [Space(15, order = 2)]

    [Tooltip("UI score text component")] 
    public TextMeshProUGUI scoreUI;
    private Animator _scoreUIAnimator;

    [Tooltip("UI balance meter slider component")]
    public Slider meterSlider;

    public RectTransform centerZoneMeter;
    public RectTransform positiveZoneMeter;
    public RectTransform negativeZoneMeter;
    public RectTransform sliderArea;
    private float _lastCenterZoneSize;
    private float _currentCenterZoneSize;
    private float _totalMeterSize;

    [Tooltip("UI day text component")]
    public TextMeshProUGUI dayUI;

    [Tooltip("OptionsMenu component")]
    public OptionsMenu optionsMenu;
    
    private List<string> _daysList;
    private int _dayCounter;

    private GameController _gameController;
    
    void Awake()
    {
        _gameController = FindObjectOfType<GameController>();
        _gameController.scoreEvent.AddListener(UpdateScoreUI);
        _scoreUIAnimator = scoreUI.GetComponent<Animator>();
        _totalMeterSize = centerZoneMeter.rect.width + positiveZoneMeter.rect.width + negativeZoneMeter.rect.width;
        _lastCenterZoneSize = centerZoneMeter.rect.width;
        _dayCounter = 0;
        _daysList = new List<string>();
        SetDaysList();
    }

    public void UpdateMeterUI(float meterValue)
    {
        //Update Meter Value on UI
        meterSlider.value = meterValue;
    }

    //Update Score value on UI
    void UpdateScoreUI(int score)
    {
        scoreUI.text = score.ToString();
    }

    public void ShowScoreUI(bool state)
    {
        scoreUI.gameObject.SetActive(state);
    }

    public void PopScoreUI()
    {
        _scoreUIAnimator.SetTrigger("Pop");
    }

    public void IdleScoreUI(bool state)
    {
        _scoreUIAnimator.SetBool("Idle", state);
    }

    public void UpdateDayUI()
    {
        if (_dayCounter == 7)
        {
            _dayCounter = 0;
        }
        dayUI.text = _daysList[_dayCounter];
        _dayCounter++;
    }

    public void ShowDayUI(bool state)
    {
        dayUI.gameObject.SetActive(state);
    }
    
    public void SetMeterUI(float currentSpread)
    {
        _currentCenterZoneSize = (_totalMeterSize * currentSpread * 2) / 2;
        float centerSizeDifference = _currentCenterZoneSize - _lastCenterZoneSize;
        centerZoneMeter.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _currentCenterZoneSize);
        positiveZoneMeter.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, positiveZoneMeter.rect.width - (centerSizeDifference/2));
        negativeZoneMeter.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, negativeZoneMeter.rect.width - (centerSizeDifference/2));
        sliderArea.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _totalMeterSize);
        centerZoneMeter.ForceUpdateRectTransforms();
        positiveZoneMeter.ForceUpdateRectTransforms();
        negativeZoneMeter.ForceUpdateRectTransforms();
        sliderArea.ForceUpdateRectTransforms();
        _lastCenterZoneSize = _currentCenterZoneSize;
    }
    
    void SetDaysList()
    {
        _daysList.Add("monday");
        _daysList.Add("tuesday");
        _daysList.Add("wednesday");
        _daysList.Add("thursday");
        _daysList.Add("friday");
        _daysList.Add("saturday");
        _daysList.Add("sunday");
    }

    private void OnDisable()
    {
        _gameController.scoreEvent.RemoveListener(UpdateScoreUI);
    }

    public void ShowOptionMenu(bool state)
    {
        optionsMenu.Show(state);
    }
}
