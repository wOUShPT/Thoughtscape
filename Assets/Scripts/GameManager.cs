using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Tooltip("SpawnManager script component")]
    public SpawnManager _spawnManager;

    [Tooltip("UI remaining time text component")]
    public Text remainingTimeText;

    [Tooltip("UI score text component")] public Text scoreValueText;
    [Tooltip("UI combo text component")] public Text scoreIncrementText;

    [Tooltip("Initial remaining time in seconds")]
    public float remainingTimeSeconds;

    [Tooltip("UI balance meter slider component")]
    public Slider _balanceMeter;

    public Volume postProcessingVolume;

    private WhiteBalance _whiteBalance;
    private Vignette _vignette;

    [Tooltip("Mood balance increase/decrease default speed in units/second")]
    public float defaultBalanceMoveSpeed;

    private Color _scoreIncrementTextColor;
    private float _timerSeconds;
    private int _score;
    private float _balance;
    private float _balanceComboTimer;
    private int _balanceComboMultiplier;
    private float _balanceMoveSpeed;
    private float _balanceMoveSpeedMultiplier;
    private int _balanceMoveSpeedComboMultiplier;
    private float _balanceSign;
    private float _lastBalanceSign;
    private float _scoreIncrementValue;
    private int _scoreIncrementCombo;
    private int _onMissDropSpeedMultiplier;
    
    void Awake()
    {
        //Set timer to default (zero)
        _timerSeconds = 0;

        //Sets score to default (zero)
        _score = 0;
        _scoreIncrementValue = 0;
        _scoreIncrementCombo = 1;
        scoreValueText.text = 0.ToString();

        //Sets initial mood balance sign, speed, and balance
        _balanceSign = -1;
        _balanceMoveSpeed = defaultBalanceMoveSpeed;
        _balanceMoveSpeedMultiplier = 3f;
        _balance = 0;

        _balanceMeter.value = 0;
        
        _onMissDropSpeedMultiplier = 1;

        _scoreIncrementTextColor = scoreIncrementText.color;
        scoreIncrementText.color = new Color(scoreIncrementText.color.r,scoreIncrementText.color.g, scoreIncrementText.color.b, 0);
        
        //Sets the "Managers" gameobject on hierarchy as a parent (this matters if you load the game from _preload scene)
        if (GameObject.FindGameObjectWithTag("Managers"))
        {
            transform.SetParent(GameObject.FindGameObjectWithTag("Managers").transform);
        }

        postProcessingVolume.profile.TryGet(out _whiteBalance);
        postProcessingVolume.profile.TryGet(out _vignette);
        _whiteBalance.temperature.min = -40;
        _whiteBalance.temperature.max = 40;
        _vignette.intensity.min = 0;
        _vignette.intensity.max = 0.5f;
    }

    void Update()
    {
        //Remaining time values update and set on UI
        _timerSeconds += Time.deltaTime;
        remainingTimeSeconds = Mathf.Clamp(remainingTimeSeconds - Time.deltaTime, 0, Mathf.Infinity);
        TimeSpan time = TimeSpan.FromSeconds(remainingTimeSeconds);
        remainingTimeText.text = time.ToString(@"mm\:ss");



        //Sets balance value 
        if (_balance < 0.20f && _balance > -0.20f)
        {
            _balanceComboTimer += Time.deltaTime;
            if (_balanceComboTimer > 5)
            {
                remainingTimeSeconds += 10;
                _score += 10;
                _balanceComboTimer = 0;
                _balanceComboMultiplier++;
                _spawnManager.SetDropSpeed(_balanceComboMultiplier);

                scoreValueText.text = _score.ToString();
            }

            _balanceMoveSpeedMultiplier = 1.5f * _scoreIncrementCombo;
            _onMissDropSpeedMultiplier = 0;
        }
        else
        {
            _balanceComboTimer = 0;
            _balanceMoveSpeedMultiplier = (1 - Mathf.Abs(_balance))  * _scoreIncrementCombo;
            _balanceComboMultiplier = 1;
            _spawnManager.SetDropSpeed(_balanceComboMultiplier);
            _spawnManager.SetDropSpeed(_onMissDropSpeedMultiplier);
            _whiteBalance.temperature.value = (((Mathf.Sign(_balance))*Mathf.Abs(_balance - Mathf.Sign(_balance) * 0.02f)) * 48) -8*Mathf.Sign(_balance);
            Debug.Log(_whiteBalance.temperature.value);
        }
        
        _balance = Mathf.Lerp(_balance, _scoreIncrementValue + _balance, _balanceMoveSpeed * _balanceMoveSpeedMultiplier * Time.deltaTime);
        //_balance += _balanceSign * _balanceMoveSpeed * _balanceMoveSpeedMultiplier * Time.deltaTime;
        _balanceMeter.value = _balance;
        
        _vignette.intensity.value = (Mathf.Abs(_balance) * 0.5f);
    }

    //sets the score based on increment or decrement passed through 
    public void Score(float value)
    {
        _onMissDropSpeedMultiplier = 0;
        if (value == _scoreIncrementValue)
        {
            _scoreIncrementCombo = Mathf.Clamp(_scoreIncrementCombo + 1, 1, 5);
            Debug.Log(_scoreIncrementCombo.ToString());
        }
        else
        {
            _balanceSign = Mathf.Sign(value);
            _scoreIncrementCombo = 1;
        }
        _scoreIncrementValue = value;
    }

    
    //Change Speed based on the number of missed thoughts
    public void OnMissChangeDropSpeed()
    {
        _onMissDropSpeedMultiplier = Mathf.Clamp(_onMissDropSpeedMultiplier - 1,-4, 1);
        Debug.Log(_onMissDropSpeedMultiplier.ToString());
    }
}
