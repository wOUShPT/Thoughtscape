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
    private MeterUI _meterUI;

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

    public void UpdateDayUI()
    {
        dayUI.text = _daysList[_dayCounter];
        dayUI.gameObject.SetActive(true);
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
    
   
}
