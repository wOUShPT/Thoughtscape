using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [FormerlySerializedAs("_spawnManager")] [Tooltip("SpawnManager script component")]
    public SpawnManager spawnManager;

    [FormerlySerializedAs("_levelsManager")]
    public LevelsManager levelsManager;
    
    public List<>

    [Tooltip("UI remaining time text component")]
    public Text timerUI;

    private float _timer;
    private bool _canStartGame;
    private ScoreEvent _scoreEvent;

    //-------------------------------------------------------------------------------------------------------------

    [Tooltip("UI score text component")] public Text scoreUI;

    private int _score;
    private float _meterIncrementValue;
    private float _lastMeterIncrementValue;
    private float _scoreIncrementCombo;

    //-------------------------------------------------------------------------------------------------------------

    [Tooltip("UI balance meter slider component")]
    public Slider meterUI;

    [FormerlySerializedAs("defaultBalanceMoveSpeed")] [Tooltip("Mood balance increase/decrease default speed in units/second")]
    public float defaultMeterMoveSpeed;

    private float _meterValue;
    private bool _isMovingUp;
    private float _meterComboTimer;
    private int _meterComboMultiplier;
    private float _meterMoveSpeed;
    private float _meterMoveSpeedMultiplier;
    private int _meterMoveSpeedComboMultiplier;
    private float _edgeTimer;

    //-------------------------------------------------------------------------------------------------------------

    [Tooltip("Transform component of water wave gameobject")]
    public Transform waveTransform;

    [Tooltip("Water level rise speed in units/second")]
    public float waterLevelRiseSpeed;

    [Tooltip("Water level drop speed in units/second")]
    public float waterLevelDropSpeed;

    private bool _canWaterRise;
    private bool _canWaterLow;

    //-------------------------------------------------------------------------------------------------------------

    [Tooltip("PostProcessing Volume component")]
    public Volume postProcessingVolume;

    private WhiteBalance _whiteBalance;
    private Vignette _vignette;

    //-------------------------------------------------------------------------------------------------------------

    private Camera _mainCamera;
    private Vector3 _screenBordersCoords;

    public Vector3 ScreenBordersCoords
    {
        get { return _screenBordersCoords; }
        private set { _screenBordersCoords = Vector3.zero; }
    }

    //-------------------------------------------------------------------------------------------------------------

    void Awake()
    {
        _mainCamera = FindObjectOfType<Camera>();

        //Get screen size width and height in pixels and convert to world units
        _screenBordersCoords = _mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        //Sets timer and score related default values
        _canStartGame = false;
        _timer = 0;
        _score = 0;
        _scoreEvent = new ScoreEvent();
        _scoreEvent.AddListener(levelsManager.LevelUp);
        _meterIncrementValue = 0;
        _lastMeterIncrementValue = 0;
        _scoreIncrementCombo = 1;
        scoreUI.text = 0.ToString();

        //Sets start meter speed, multiplier, internal value and UI value
        _meterMoveSpeed = defaultMeterMoveSpeed;
        _meterMoveSpeedMultiplier = 3f;
        _meterValue = 0;
        _edgeTimer = 0;
        meterUI.value = 0;

        //Get postprocessing filters and set base values
        postProcessingVolume.profile.TryGet(out _whiteBalance);
        postProcessingVolume.profile.TryGet(out _vignette);
        _whiteBalance.temperature.min = -40;
        _whiteBalance.temperature.max = 40;
        _vignette.intensity.min = 0;
        _vignette.intensity.max = 0.5f;

        //Start Water Update Loop
        _canWaterLow = false;
        _canWaterRise = false;
        StartCoroutine(UpdateWaterLevel());
    }

    private void Start()
    {
        //Sets the "Managers" gameobject on hierarchy as a parent (this matters if you load the game from _preload scene)
        if (GameObject.FindGameObjectWithTag("Managers"))
        {
            transform.SetParent(GameObject.FindGameObjectWithTag("Managers").transform);
        }
    }

    void Update()
    {
        //Check if the player caught a thought and if it's true begins the timer, score and meter logic
        if (_canStartGame)
        {
            //Timer Update
            _timer += Time.deltaTime;

            //Sets balance value 
            if (_meterValue < 0.20f && _meterValue > -0.20f)
            {
                _canWaterRise = false;
                _canWaterLow = true;
                _meterComboTimer += Time.deltaTime;
                if (_meterComboTimer > 5)
                {
                    _score += 10;
                    _meterComboTimer = 0;
                    _meterComboMultiplier++;
                    spawnManager.SetHorizontalForceIncrement(_meterComboMultiplier * 0.1f);
                    spawnManager.SetDropSpeed(_meterComboMultiplier);
                    _scoreEvent.Invoke(_score);
                }

                _meterMoveSpeedMultiplier = (1f - Mathf.Abs(_meterValue) * 0.4f) * 1.5f * _scoreIncrementCombo;
            }
            else
            {
                _canWaterRise = true;
                _canWaterLow = false;
                _meterComboTimer = 0;
                if (_isMovingUp && Mathf.Sign(_meterValue) == -1 || !_isMovingUp && Mathf.Sign(_meterValue) == 1)
                {
                    _meterMoveSpeedMultiplier = Mathf.Abs(_meterValue) * 3f * _scoreIncrementCombo;
                }
                else
                {
                    _meterMoveSpeedMultiplier = (1.1f - Mathf.Abs(_meterValue)) * 3f * _scoreIncrementCombo;
                }

                _meterComboMultiplier = 1;
                spawnManager.SetDropSpeed(_meterComboMultiplier);
                spawnManager.SetHorizontalForceIncrement(0);
                UpdateWhiteBalanceFilter();
            }

        }

        //_meterValue = Mathf.Lerp(_meterValue, _scoreIncrementValue + _meterValue, _meterMoveSpeed * _meterMoveSpeedMultiplier * Time.deltaTime);
        _meterValue += _meterIncrementValue * _meterMoveSpeed * _meterMoveSpeedMultiplier * Time.deltaTime;
        _meterValue = Mathf.Clamp(_meterValue, -1f, 1f);

        if (Mathf.Approximately(waveTransform.position.y, -ScreenBordersCoords.y + 0.8f))
        {
            GameOver();
        }

        if (Mathf.Abs(_meterValue) == 1)
        {
            _edgeTimer += Time.deltaTime;
            UpdateVignetteFilter();
            if (_edgeTimer > 5)
            {
                GameOver();
            }
        }
        else
        {
            _edgeTimer = 0;
        }


        //UI Update
        UpdateUI();
    }

    //sets the meter multipliers and score combos based on increment or decrement passed through 
    public void OnScoreEvent(float value)
    {
        if (_lastMeterIncrementValue == 0)
        {
            _canStartGame = true;
        }

        if (value == -10)
        {
            GameOver();
        }

        _isMovingUp = Mathf.Sign(value) == 1;

        if (Mathf.Sign(_lastMeterIncrementValue) == Mathf.Sign(value))
        {
            if (Mathf.Abs(value) < Mathf.Abs(_meterIncrementValue))
            {
                _meterIncrementValue = _lastMeterIncrementValue;
            }
            else if (Mathf.Abs(value) > Mathf.Abs(_meterIncrementValue))
            {
                _meterIncrementValue = value;
            }
            else //if value == _meterIncrementValue
            {
                _scoreIncrementCombo = Mathf.Clamp(_scoreIncrementCombo + 0.5f, 1, 10);
                _meterIncrementValue = value;
            }
        }
        else
        {
            _meterIncrementValue = value;
            _scoreIncrementCombo = 1;
        }

        _lastMeterIncrementValue = value;
    }

    //Change Speed based on the number of missed thoughts
    public void OnMissEvent()
    {

    }

    private void GameOver()
    {
        SceneManager.LoadGameScene();
    }

    //---------------- PostProcessing, Visual Effects and UI Related Functions ------------------------------------

    //Updates post processing vignette filter based on the meter balance values
    void UpdateVignetteFilter()
    {
        _vignette.intensity.value = (Mathf.Abs(_meterValue) * 0.5f);
    }

    //Updates post processing white balance filter based on the meter balance values
    void UpdateWhiteBalanceFilter()
    {
        _whiteBalance.temperature.value += Mathf.Sign(_meterValue) * 4 * Time.deltaTime;
        //_whiteBalance.temperature.value = (((Mathf.Sign(_meterValue))*Mathf.Abs(_meterValue - Mathf.Sign(_meterValue) * 0.02f)) * 48) -8*Mathf.Sign(_meterValue);
    }

    void UpdateUI()
    {
        //Timer UI
        TimeSpan time = TimeSpan.FromSeconds(_timer);
        timerUI.text = time.ToString(@"mm\:ss");

        //Score UI
        scoreUI.text = _score.ToString();

        //Meter UI
        meterUI.value = _meterValue;
    }

    //Water level Update loop
    IEnumerator UpdateWaterLevel()
    {
        waveTransform.position =
            new Vector3(waveTransform.position.x, -_screenBordersCoords.y - 4, waveTransform.position.z);
        while (true)
        {
            if (_canWaterLow)
            {
                waveTransform.position = new Vector3(waveTransform.position.x,
                    Mathf.Clamp(waveTransform.position.y - waterLevelDropSpeed * Time.deltaTime,
                        -ScreenBordersCoords.y - 4, -ScreenBordersCoords.y + 0.8f)
                    , waveTransform.position.z);
                yield return null;
            }

            if (_canWaterRise)
            {
                waveTransform.position = new Vector3(waveTransform.position.x,
                    Mathf.Clamp(waveTransform.position.y + waterLevelRiseSpeed * Time.deltaTime,
                        -ScreenBordersCoords.y - 4, -ScreenBordersCoords.y + 0.8f)
                    , waveTransform.position.z);
            }

            yield return null;
        }
    }

    public void SetValues(int levelIndex)
    {
        switch (levelIndex)
        {
            case 1:
                
                break;

            case 2:

                break;

            case 3:

                break;

            case 4:

                break;
        }
    }

    public class ScoreEvent : UnityEvent<int>
    {
        
    }
}

