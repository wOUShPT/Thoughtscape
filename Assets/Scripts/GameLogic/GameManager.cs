﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using Random = Unity.Mathematics.Random;

public class GameManager : MonoBehaviour
{
    private SceneManager _sceneManager;

    #region Level Progression Related Declaration

    [Space(30, order = 0)]
    [Header("Level Progression Settings", order = 1)]
    [Space(15, order = 2)]

    [Tooltip("List of level related parameters data containers")]
    public List<LevelParametersScriptableObject> levelParametersDataList;
    [Tooltip("List of score goals to level up")]
    public List<int> scoreGoalsToLevelUp;
    private int _levelIndex;
    public float levelTransitionTimeDuration;
    private float _levelTransitionTimer;
    
    #endregion
    
    //-------------------------------------------------------------------------------------------------------------
    
    #region Spawn Related Declaration
    
    [Space(30, order = 0)]
    [Header("Spawn Settings", order = 1)]
    [Space(15, order = 2)]

    [Tooltip("ThoughtPrefab asset in project")]
    public ThoughtBehaviour thoughtPrefab;
    private List<ThoughtBehaviour> _thoughtsPool;

    [Space(15, order = 0)]
    [Tooltip("List of thoughts attributes data containers")]
    public List<ThoughtsAttributesScriptableObject> thoughtsAttributesList;
    private List<float> _thoughtCurrentSpawnRatesList;

    [Space(15, order = 0)]
    [Tooltip("Thoughts object pool maximum capacity (InstantiatedOnLoad)")]
    public int thoughtsPoolCapacity;

    [Space(15, order = 0)]
    [Tooltip("Can spawn thoughts?")]
    public bool canSpawn;

    [Tooltip("Minimum time between spawns in seconds")]
    public float minTimeBetweenSpawns;
    [Tooltip("Maximum time between spawns in seconds")]
    public float maxTimeBetweenSpawns;
    
    private float _randomTimeInterval;

    #endregion
    
    //-------------------------------------------------------------------------------------------------------------

    #region Current Game Related Parameters Declaration
    
    [Space(30, order = 0)]
    [Header("Current Level Settings", order = 1)]
    [Space(15, order = 2)]
    
    [Tooltip("Meter current move speed")]
    public float meterCurrentMoveSpeed;

    private float _timer;
    private bool _canStartGame;
    private int _scoreValue;
    private float _scoreComboTimer;
    private float _scoreTimeInterval;
    private int _scoreComboMultiplier;
    private float _meterIncrementValue;
    private float _lastMeterIncrementValue;
    private float _scoreIncrementCombo;

    private float _currentMeterSpreadValue;
    private float _meterValue;
    private bool _isMovingUp;
    private float _meterComboTimer;
    private int _meterComboMultiplier;
    private float _meterMoveSpeed;
    private float _meterMoveSpeedMultiplier;
    private float _meterLimitsTimer;
    private float _meterLimitsTimeToDeath;
    
    [Space(15, order = 0)]
    [Tooltip("Transform component of water wave gameobject")]
    public Transform waterWaveTransform;

    private Vector3 _waterLevelDefaultPosition;

    [Tooltip("Water level rise speed in units/second")]
    public float waterLevelRiseSpeed;

    [Tooltip("Water level drop speed in units/second")]
    public float waterLevelDropSpeed;

    private bool _canWaterRise;
    private bool _canWaterLow;

    #endregion
    
    //-------------------------------------------------------------------------------------------------------------
    
    #region UI Related Parameters Declaration
   
    [Space(30, order = 0)]
    [Header("UI Settings", order = 1)]
    [Space(15, order = 2)]

    [Tooltip("UI remaining time text component")]
    public Text timerUI;
    
    [Tooltip("UI score text component")] 
    public Text scoreUI;

    [Tooltip("UI score increment text component")]
    public Text scoreIncrementUI;
    
    [Tooltip("UI balance meter slider component")]
    public Slider meterSlider;
    private MeterUI _meterUI;

    [Tooltip("UI day text component")]
    public Text dayUI;
    
    #endregion

    //-------------------------------------------------------------------------------------------------------------

    #region Post Processing Parameters Declaration

    [Space(30, order = 0)]
    [Header("Post Processing and Particle System", order = 1)]
    [Space(15, order = 2)]
    
    [Tooltip("Post Processing Volume component")]
    public Volume postProcessingVolume;
    private WhiteBalance _whiteBalance;
    private Vignette _vignette;
    private ChromaticAberration _chromaticAberration;
    public float chromaticAberrationFeedbackEffectTime;
    private int _chromaticAberrationSign;

    public ParticleSystem showerParticleSystem;
    public bool canSpawnParticles;

    #endregion
    
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
        _sceneManager = FindObjectOfType<SceneManager>();
        
        _mainCamera = FindObjectOfType<Camera>();
        
        _meterUI = meterSlider.gameObject.GetComponent<MeterUI>();

        //Get screen size width and height in pixels and convert to world units
        _screenBordersCoords = _mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        //Instantiate thoughts and create a pool based on a pre-established capacity
        _thoughtsPool = new List<ThoughtBehaviour>();
        for (int i = 0; i < thoughtsPoolCapacity; i++)
        {
            InstantiateThought();
        }

        _thoughtCurrentSpawnRatesList = new List<float>();
        for (int i = 0; i < thoughtsAttributesList.Count; i++)
        {
            _thoughtCurrentSpawnRatesList.Add(0);
        }

        canSpawn = false;

        //Initialize the first level
        _levelIndex = 0;
        SetLevelParameters();
        StartCoroutine(LevelTransition());

        //Get postprocessing filters and set base values
        postProcessingVolume.profile.TryGet(out _whiteBalance);
        postProcessingVolume.profile.TryGet(out _vignette);
        postProcessingVolume.profile.TryGet(out _chromaticAberration);
        _whiteBalance.temperature.min = -40;
        _whiteBalance.temperature.max = 40;
        _vignette.intensity.min = 0.2f;
        _vignette.intensity.max = 0.5f;
        _chromaticAberration.intensity.min = 0f;
        _chromaticAberration.intensity.max = 1f;
        _chromaticAberrationSign = 1;

        //Start Water Update Loop
        _canWaterLow = false;
        _canWaterRise = false;
        _waterLevelDefaultPosition = waterWaveTransform.position;
        StartCoroutine(UpdateWaterLevel());
        
        //Set score and game timer to zero
        _timer = 0;
        _scoreValue = 0;
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

            //Checks if the player it's in the center zone of the meter
            if (_meterValue < _currentMeterSpreadValue && _meterValue > -_currentMeterSpreadValue)
            {
                _canWaterRise = false;
                _canWaterLow = true;
                _meterComboTimer += Time.deltaTime;
                _scoreComboTimer += Time.deltaTime;

                if (_scoreComboTimer > Mathf.Clamp(_scoreTimeInterval - (_scoreComboMultiplier), 1f, 5f))
                {
                    _scoreComboMultiplier++;
                    _scoreValue += 10;
                    _scoreComboTimer = 0;
                    StartCoroutine(UpdateScoreIncrementUI());
                    CheckIfLevelUp();
                }
                
                if (_meterComboTimer > 5)
                {
                    _meterComboTimer = 0;
                    _meterComboMultiplier++;
                    SetHorizontalForceComboIncrement(_meterComboMultiplier * 0.1f);
                    SetDropSpeed(levelParametersDataList[_levelIndex].dropSpeed + 0.1f * _meterComboMultiplier);
                }
                
                //Set Spawn Rates based on meter position (zone)
                for (int i = 0; i < levelParametersDataList[_levelIndex].thoughtSpawnRatesList.Count; i++)
                {
                    _thoughtCurrentSpawnRatesList[i] = levelParametersDataList[_levelIndex]
                            .thoughtSpawnRatesList[i].centerMeterZoneSpawnRatePercentage;
                }

                _meterMoveSpeedMultiplier = (1f - Mathf.Abs(_meterValue) * _currentMeterSpreadValue*2) * 1.2f * _scoreIncrementCombo;
            }
            else
            {
                _canWaterRise = true;
                _canWaterLow = false;
                _meterComboTimer = 0;
                _scoreComboTimer = 0;
                _scoreComboMultiplier = 0;
                _meterComboMultiplier = 0;
                
                SetDropSpeed(levelParametersDataList[_levelIndex].dropSpeed);
                SetHorizontalForceComboIncrement(0);
                
                if (_isMovingUp && Mathf.Sign(_meterValue) == -1 || !_isMovingUp && Mathf.Sign(_meterValue) == 1)
                {
                    _meterMoveSpeedMultiplier = Mathf.Abs(_meterValue) * 3f * _scoreIncrementCombo;
                }
                else
                {
                    _meterMoveSpeedMultiplier = (1.1f - Mathf.Abs(_meterValue)) * 3f * _scoreIncrementCombo;
                }
                
                //Set Spawn Rates based on meter position (zone)
                for (int i = 0; i < levelParametersDataList[_levelIndex].thoughtSpawnRatesList.Count; i++)
                {
                    if (_meterValue <= -0.2f)
                            _thoughtCurrentSpawnRatesList[i] = levelParametersDataList[_levelIndex]
                                .thoughtSpawnRatesList[i].negativeZoneMeterSpawnRatePercentage;
                    else
                    {
                            _thoughtCurrentSpawnRatesList[i] = levelParametersDataList[_levelIndex]
                                .thoughtSpawnRatesList[i].positiveZoneMeterSpawnRatePercentage;
                    }
                }

                if (Mathf.Abs(_meterValue) == 1)
                {
                    _meterLimitsTimer += Time.deltaTime;
                    if (_meterLimitsTimer > _meterLimitsTimeToDeath)
                    {
                        GameOver();
                    }
                    
                    //Set Spawn Rates based on meter position (zone)
                    for (int i = 0; i < levelParametersDataList[_levelIndex].thoughtSpawnRatesList.Count; i++)
                    { 
                            _thoughtCurrentSpawnRatesList[i] = levelParametersDataList[_levelIndex]
                                .thoughtSpawnRatesList[i].limitZoneMeterSpawnRatePercentage;
                    }
                }

                _meterLimitsTimer = 0;
                
                UpdateWhiteBalanceFilter();
            }
        }
        else
        {
            _meterValue = 0;
            _meterIncrementValue = 0;
            _meterMoveSpeedMultiplier = 0;
            _lastMeterIncrementValue = 0;
            _meterComboMultiplier = 0;
        }

        _meterValue += _meterIncrementValue * _meterMoveSpeed * _meterMoveSpeedMultiplier * Time.deltaTime;
        _meterValue = Mathf.Clamp(_meterValue, -1f, 1f);

        if (Mathf.Approximately(waterWaveTransform.position.y, -ScreenBordersCoords.y + 0.8f))
        {
            GameOver();
        }
        
        UpdateVignetteFilter();

        //UI Update
        UpdateUI();
    }
    
    //Instantiate a new thought on the scene
    public void InstantiateThought()
    {
        ThoughtBehaviour thought = Instantiate(thoughtPrefab);
        _thoughtsPool.Add(thought);
        thought.gameObject.SetActive(false);
    }

    IEnumerator SpawnThoughts()
    {
        while (canSpawn)
        {
            for (int i = 0; i < _thoughtsPool.Count; i++)
            {
                if (!_thoughtsPool[i].gameObject.activeSelf)
                {
                    ReUseThought(_thoughtsPool[i]);
                    break;
                }
            }
            GenerateRandomTimeInterval();
            yield return new WaitForSeconds(_randomTimeInterval);
            yield return null;
        }
    }

    //It passes a thought as an argument, activates it, resets the behaviour component and sets his spawn position using the screen borders as reference
    public void ReUseThought(ThoughtBehaviour thought)
    {
        thought.gameObject.SetActive(true);
        int randomIndex = GetWeightedRandomIndex();
        thought.ResetBehaviour(randomIndex);
        thought.transform.position = new Vector3(UnityEngine.Random.Range(-ScreenBordersCoords.x+0.5f, ScreenBordersCoords.x-0.5f),ScreenBordersCoords.y+2,0);
    }

    public void DeSpawnThought(ThoughtBehaviour thought)
    {
        if (thought.isActiveAndEnabled)
        {
            thought.Fade();
        }
    }

    //Generates a random time frame between two given values
    void GenerateRandomTimeInterval()
    {
        _randomTimeInterval = UnityEngine.Random.Range(minTimeBetweenSpawns, maxTimeBetweenSpawns);
    }

    public void SetDropSpeed(float dropSpeedValue)
    {
        foreach (var thought in _thoughtsPool)
        {
            thought.dropSpeed = dropSpeedValue;
        }   
    }

    public void SetHorizontalForceComboIncrement(float forceIncrement)
    {
        foreach (var thought in _thoughtsPool)
        {
            thought.horizontalForceComboIncrementValue = forceIncrement;
        }
    }
    
    void AllowThoughtSpawn(string category, bool canSpawn)
    {
        foreach (var though in thoughtsAttributesList)
        {
            if (though.category == category)
            {
                though.canSpawn = canSpawn;
            }
        }
    }

    // Receives a list of probabilities (percentages) and returns a random index of it based on the percentages
    public int GetWeightedRandomIndex()
    {
        float sum = 0;
        for (int i = 0; i < _thoughtCurrentSpawnRatesList.Count; i++)
        {
            if (thoughtsAttributesList[i].canSpawn)
            {
                sum += _thoughtCurrentSpawnRatesList[i];
            }
        }
        float randomWeight = 0;
        
        do
        {
            if (sum == 0)
            {
                return 0;
            }
            
            randomWeight = UnityEngine.Random.Range(0, sum);
        } 
        while (randomWeight == sum);
            
        for(int i = 0; i < thoughtsAttributesList.Count; i++)
        {
            if (thoughtsAttributesList[i].canSpawn)
            {
                if (randomWeight < _thoughtCurrentSpawnRatesList[i])
                {
                    return i;
                }
                randomWeight -= _thoughtCurrentSpawnRatesList[i];
            }
        }

        return 0;
    }
    
    
    //sets the meter multipliers and score combos based on increment or decrement passed through 
    public void OnCatchEvent(float value)
    {
        StartCoroutine(ChromaticAberrationFeedback());
        if (_lastMeterIncrementValue == 0)
        {
            _canStartGame = true;
        }

        if (value == -10)
        {
            GameOver();
        }

        _isMovingUp = Mathf.Sign(value) == 1;

        if (Mathf.Sign(_lastMeterIncrementValue) == Mathf.Sign(value) && value != 10)
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
            _lastMeterIncrementValue = value;
        }
        else
        {
            if (value == 10)
            {
                _meterValue = 0;
                _scoreIncrementCombo = 1;
                return;
            }
            _meterIncrementValue = value;
            _scoreIncrementCombo = 1;
            _lastMeterIncrementValue = value;
        }
    }

    private void GameOver()
    {
        _sceneManager.LoadGameScene();
    }

    //---------------- PostProcessing, Visual Effects and UI Related Functions ------------------------------------

    //Updates post processing vignette filter based on the meter balance values
    void UpdateVignetteFilter()
    {
        if (Mathf.Abs(_meterValue) == 1)
        {
            _vignette.intensity.value = Mathf.Lerp(_vignette.intensity.value, _vignette.intensity.max, ((_vignette.intensity.max - _vignette.intensity.min)/_meterLimitsTimeToDeath) * Time.deltaTime);
        }
        else
        {
            _vignette.intensity.value = Mathf.Lerp(_vignette.intensity.value, _vignette.intensity.min, 2 * Time.deltaTime);
        }
        //_vignette.intensity.value = (Mathf.Abs(_meterValue) * 0.5f);
    }

    //Updates post processing white balance filter based on the meter balance values
    void UpdateWhiteBalanceFilter()
    {
        if (_meterValue < -_currentMeterSpreadValue || _meterValue > _currentMeterSpreadValue)
        {
            //_whiteBalance.temperature.value += Mathf.Sign(_meterValue) * 4 * Time.deltaTime;
            _whiteBalance.temperature.value = (((Mathf.Sign(_meterValue))*Mathf.Abs(_meterValue - Mathf.Sign(_meterValue) * _currentMeterSpreadValue)) * 48) -8*Mathf.Sign(_meterValue);
        }
        else
        {
            _whiteBalance.temperature.value = 0;
        }
    }

    public IEnumerator ChromaticAberrationFeedback()
    {
        float speed = (_chromaticAberration.intensity.max - _chromaticAberration.intensity.min) / (chromaticAberrationFeedbackEffectTime/2);
        while (_chromaticAberration.intensity.value <= _chromaticAberration.intensity.max)
        {
            _chromaticAberration.intensity.value += _chromaticAberrationSign * speed * Time.deltaTime;
            _chromaticAberration.intensity.value = Mathf.Clamp(_chromaticAberration.intensity.value, _chromaticAberration.intensity.min, _chromaticAberration.intensity.max);
            if (_chromaticAberration.intensity.value == _chromaticAberration.intensity.max)
            {
                _chromaticAberrationSign = -_chromaticAberrationSign;
            }

            if (_chromaticAberration.intensity.value == _chromaticAberration.intensity.min)
            {
                _chromaticAberrationSign = -_chromaticAberrationSign;
                break;
            }
            yield return null;
        }
    }

    void UpdateUI()
    {
        //Timer UI
        TimeSpan time = TimeSpan.FromSeconds(_timer);
        timerUI.text = time.ToString(@"mm\:ss");

        //Score UI
        scoreUI.text = _scoreValue.ToString();
        scoreIncrementUI.text = "+ 10";
        
        //Meter UI
        meterSlider.value = _meterValue;
    }

    IEnumerator UpdateScoreIncrementUI()
    {
        scoreIncrementUI.gameObject.SetActive(true);
        yield return new WaitForSeconds((_scoreTimeInterval - (_scoreComboMultiplier))/1.1f);
        scoreIncrementUI.gameObject.SetActive(false);
        yield return null;
    }

    //Water level Update loop
    IEnumerator UpdateWaterLevel()
    {
        waterWaveTransform.position =
            new Vector3(waterWaveTransform.position.x, -_screenBordersCoords.y - 4, waterWaveTransform.position.z);
        while (true)
        {
            if (_canWaterLow)
            {
                waterWaveTransform.position = new Vector3(waterWaveTransform.position.x,
                    Mathf.Clamp(waterWaveTransform.position.y - waterLevelDropSpeed * Time.deltaTime,
                        -ScreenBordersCoords.y - 4, -ScreenBordersCoords.y + 0.8f)
                    , waterWaveTransform.position.z);
                yield return null;
            }

            if (_canWaterRise)
            {
                waterWaveTransform.position = new Vector3(waterWaveTransform.position.x,
                    Mathf.Clamp(waterWaveTransform.position.y + waterLevelRiseSpeed * Time.deltaTime,
                        -ScreenBordersCoords.y - 4, -ScreenBordersCoords.y + 0.8f)
                    , waterWaveTransform.position.z);
            }

            yield return null;
        }
    }
    
    //Check if can level up
    public void CheckIfLevelUp()
    {
        if (_scoreValue == scoreGoalsToLevelUp[_levelIndex] && _levelIndex < levelParametersDataList.Count)
        {
            _levelIndex++;
            StartCoroutine(LevelTransition());
        }
    }

    IEnumerator LevelTransition()
    {
        canSpawn = false;
        dayUI.text = levelParametersDataList[_levelIndex].day;
        dayUI.gameObject.SetActive(true);
        foreach (var thought in _thoughtsPool)
        {
            DeSpawnThought(thought);
        }
        SetLevelParameters();
        showerParticleSystem.Stop();
        yield return new WaitForSeconds(levelTransitionTimeDuration);
        dayUI.gameObject.SetActive(false);
        showerParticleSystem.Play();
        canSpawn = true;
        StartCoroutine(SpawnThoughts());
        _canStartGame = true;
    }

    //Set level parameters (ex: drop speed, meter move speed, spawn ratio, etc)
    public void SetLevelParameters()
    {
        switch (_levelIndex)
        {
            case 0:

                //Sets timer and score related default values
                _canStartGame = false;
                _scoreComboTimer = 0;
                _scoreComboMultiplier = 0;
                _scoreTimeInterval = levelParametersDataList[_levelIndex].scoreBaseTime;
                _scoreIncrementCombo = 1;

                //Sets start meter speed, multiplier, internal value and UI value
                _currentMeterSpreadValue = levelParametersDataList[_levelIndex].meterCenterSpreadValue;
                _meterUI.SetMeterUI(_currentMeterSpreadValue);
                _meterMoveSpeed = levelParametersDataList[_levelIndex].meterBaseMoveSpeed;
                _meterMoveSpeedMultiplier = 0;
                _meterValue = 0;
                _meterIncrementValue = 0;
                _lastMeterIncrementValue = 0;
                _meterLimitsTimer = 0;
                _meterLimitsTimeToDeath = levelParametersDataList[_levelIndex].meterLimitsTimeToDeath;
                waterLevelDropSpeed = levelParametersDataList[_levelIndex].waterLevelDropSpeed;
                waterLevelRiseSpeed = levelParametersDataList[_levelIndex].waterLevelRiseSpeed;
                SetDropSpeed(levelParametersDataList[_levelIndex].dropSpeed);
                break;
            
            case 1:

                //Sets timer and score related default value
                 _canStartGame = false;
                _scoreComboTimer = 0;
                _scoreComboMultiplier = 0;
                _scoreTimeInterval = levelParametersDataList[_levelIndex].scoreBaseTime;
                _scoreIncrementCombo = 1;

                //Sets start meter speed, multiplier, internal value and UI value
                _currentMeterSpreadValue = levelParametersDataList[_levelIndex].meterCenterSpreadValue;
                _meterUI.SetMeterUI(_currentMeterSpreadValue);
                _meterMoveSpeed = levelParametersDataList[_levelIndex].meterBaseMoveSpeed;
                _meterMoveSpeedMultiplier = 0;
                _meterValue = 0;
                _meterIncrementValue = 0;
                _lastMeterIncrementValue = 0;
                _meterLimitsTimer = 0;
                _meterLimitsTimeToDeath = levelParametersDataList[_levelIndex].meterLimitsTimeToDeath;
                waterLevelDropSpeed = levelParametersDataList[_levelIndex].waterLevelDropSpeed;
                waterLevelRiseSpeed = levelParametersDataList[_levelIndex].waterLevelRiseSpeed;
                SetDropSpeed(levelParametersDataList[_levelIndex].dropSpeed);
                break;

            case 2:

                //Sets timer and score related default value
                _canStartGame = false;
                _scoreComboTimer = 0;
                _scoreComboMultiplier = 0;
                _scoreTimeInterval = levelParametersDataList[_levelIndex].scoreBaseTime;
                _scoreIncrementCombo = 1;

                //Sets start meter speed, multiplier, internal value and UI value
                _currentMeterSpreadValue = levelParametersDataList[_levelIndex].meterCenterSpreadValue;
                _meterUI.SetMeterUI(_currentMeterSpreadValue);
                _meterMoveSpeed = levelParametersDataList[_levelIndex].meterBaseMoveSpeed;
                _meterMoveSpeedMultiplier = 0;
                _meterValue = 0;
                _meterIncrementValue = 0;
                _lastMeterIncrementValue = 0;
                _meterLimitsTimer = 0;
                _meterLimitsTimeToDeath = levelParametersDataList[_levelIndex].meterLimitsTimeToDeath;
                waterLevelDropSpeed = levelParametersDataList[_levelIndex].waterLevelDropSpeed;
                waterLevelRiseSpeed = levelParametersDataList[_levelIndex].waterLevelRiseSpeed;
                SetDropSpeed(levelParametersDataList[_levelIndex].dropSpeed);
                break;

            case 3:

                break;

            case 4:

                break;
        }
    }
}
