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
    [Tooltip("Mood balance increase/decrease default speed in units/second")]
    public float defaultBalanceMoveSpeed;
    
    private float _balance;
    private float _balanceComboTimer;
    private int _balanceComboMultiplier;
    private float _balanceMoveSpeed;
    private float _balanceMoveSpeedMultiplier;
    private int _balanceMoveSpeedComboMultiplier;

   
    [Tooltip("Transform component of water wave gameobject")]
    public Transform waveTransform;

    public float waterLevelSpeed;
    
   
    
    private Color _scoreIncrementTextColor;
    private int _score;
    
    private float _scoreIncrementValue;
    private int _scoreIncrementCombo;
    private int _onMissDropSpeedMultiplier;
    private bool _canWaterRise;
    private bool _canWaterLow;
    
    [Tooltip("PostProcessing Volume component")]
    public Volume postProcessingVolume;
    private WhiteBalance _whiteBalance;
    private Vignette _vignette;
    
    private Camera _mainCamera;
    private Vector3 screenBordersCoords;
    
    public Vector3 ScreenBordersCoords  // property
    {
        get { return screenBordersCoords; } 
        private set { screenBordersCoords = Vector3.zero; } 
    }

    void Awake()
    {
        _mainCamera = FindObjectOfType<Camera>();
        
        //Get screen size width and height in pixels and convert to world units
        screenBordersCoords = _mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        
        //Sets score to default (zero)
        _score = 0;
        _scoreIncrementValue = 0;
        _scoreIncrementCombo = 1;
        scoreValueText.text = 0.ToString();

        //Sets initial mood balance sign, speed, and balance
        _balanceMoveSpeed = defaultBalanceMoveSpeed;
        _balanceMoveSpeedMultiplier = 3f;
        _balance = 0;
        _canWaterLow = false;

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

    private void Start()
    {
        
        StartCoroutine(UpdateWaterLevel());
    }

    void Update()
    {
        //Remaining time values update
        remainingTimeSeconds = Mathf.Clamp(remainingTimeSeconds - Time.deltaTime, 0, Mathf.Infinity);

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
            UpdateWhiteBalanceFilter();
            
        }
        
        _balance = Mathf.Lerp(_balance, _scoreIncrementValue + _balance, _balanceMoveSpeed * _balanceMoveSpeedMultiplier * Time.deltaTime);
        
        //_balance += _balanceSign * _balanceMoveSpeed * _balanceMoveSpeedMultiplier * Time.deltaTime;

        UpdateVignetteFilter();
        
        //UI Update
        UpdateUI();
    }

    //sets the score based on increment or decrement passed through 
    public void OnScoreEvent(float value)
    {
        _onMissDropSpeedMultiplier = 0;
        _canWaterRise = false;
        if (value == _scoreIncrementValue)
        {
            _scoreIncrementCombo = Mathf.Clamp(_scoreIncrementCombo + 1, 1, 5);
        }
        else
        {
            _scoreIncrementCombo = 1;
        }
        _scoreIncrementValue = value;
    }

    
    //Change Speed based on the number of missed thoughts
    public void OnMissEvent()
    {
        _onMissDropSpeedMultiplier = Mathf.Clamp(_onMissDropSpeedMultiplier - 1,-4, 1);
        _canWaterRise = true;
        Debug.Log(_onMissDropSpeedMultiplier.ToString());
    }

    
    
    //----------------PostProcessing Effects and UI------------------------------------
    
    //Updates post processing vignette filter based on the meter balance values
    void UpdateVignetteFilter()
    {
        _vignette.intensity.value = (Mathf.Abs(_balance) * 0.5f);
    }

    //Updates post processing white balance filter based on the meter balance values
    void UpdateWhiteBalanceFilter()
    {
        _whiteBalance.temperature.value = (((Mathf.Sign(_balance))*Mathf.Abs(_balance - Mathf.Sign(_balance) * 0.02f)) * 48) -8*Mathf.Sign(_balance);
    }

    void UpdateUI()
    {
        //Remaining Time
        TimeSpan time = TimeSpan.FromSeconds(remainingTimeSeconds);
        remainingTimeText.text = time.ToString(@"mm\:ss");
        
        //score
        scoreValueText.text = _score.ToString();
        
        //Balance Meter
        _balanceMeter.value = _balance;
    }

    IEnumerator UpdateWaterLevel()
    {
        waveTransform.position = new Vector3(waveTransform.position.x, -screenBordersCoords.y - 4, waveTransform.position.z);
        while (true)
        {
            if (_canWaterLow)
            {
                
            }
            if (_canWaterRise)
            {
                waveTransform.position = new Vector3(waveTransform.position.x, Mathf.Clamp(waveTransform.position.y + waterLevelSpeed * Time.deltaTime, -ScreenBordersCoords.y - 4, -ScreenBordersCoords.y + 0.8f)
                    , waveTransform.position.z);
            }
            yield return null;
        }
    }
}
