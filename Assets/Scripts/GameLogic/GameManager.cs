using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
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
    private List<float> _thoughtsAttributesListSpawnRates;

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
    private float _spawnTimer;
    
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
    private float _meterIncrementValue;
    private float _lastMeterIncrementValue;
    private float _scoreIncrementCombo;

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
    
    [Tooltip("UI balance meter slider component")]
    public Slider meterUI;

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

    private void Start()
    {
        //Sets the "Managers" gameobject on hierarchy as a parent (this matters if you load the game from _preload scene)
        if (GameObject.FindGameObjectWithTag("Managers"))
        {
            transform.SetParent(GameObject.FindGameObjectWithTag("Managers").transform);
        }
    }

    void Awake()
    {
        _sceneManager = FindObjectOfType<SceneManager>();
        
        _mainCamera = FindObjectOfType<Camera>();

        //Get screen size width and height in pixels and convert to world units
        _screenBordersCoords = _mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        //Instantiate thoughts and create a pool based on a pre-established capacity
        _thoughtsPool = new List<ThoughtBehaviour>();
        for (int i = 0; i < thoughtsPoolCapacity; i++)
        {
            InstantiateThought();
        }
        
        //Set the Thought attributes spawn rate percentages List
        _thoughtsAttributesListSpawnRates = new List<float>();
        foreach (var thoughAttribute in thoughtsAttributesList)
        {
            _thoughtsAttributesListSpawnRates.Add(thoughAttribute.spawnRatePercentage);
        }

        canSpawn = false;
        
        //Initialize the first level
        _levelIndex = 0;
        SetLevelParameters();
        StartCoroutine(LevelTransition());
        
        //Get postprocessing filters and set base values
        postProcessingVolume.profile.TryGet(out _whiteBalance);
        postProcessingVolume.profile.TryGet(out _vignette);
        _whiteBalance.temperature.min = -40;
        _whiteBalance.temperature.max = 40;
        _vignette.intensity.min = 0.2f;
        _vignette.intensity.max = 0.5f;

        //Start Water Update Loop
        _canWaterLow = false;
        _canWaterRise = false;
        _waterLevelDefaultPosition = waterWaveTransform.position;
        StartCoroutine(UpdateWaterLevel());
    }

    void Update()
    {
        if (canSpawn)
        {
            //when the timer surpasses a random generated time frame, search through the pool for an available thought to reuse and spawn it
            if (_spawnTimer > _randomTimeInterval)
            {
                GenerateRandomTimeInterval();
                _spawnTimer = 0;
                for (int i = 0; i < _thoughtsPool.Count; i++)
                {
                    if (!_thoughtsPool[i].gameObject.activeSelf)
                    {
                        ReSpawnThought(_thoughtsPool[i]);
                        return;
                    }
                }
            }
            _spawnTimer += Time.deltaTime;
        }

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
                    _scoreValue += 10;
                    _meterComboTimer = 0;
                    _meterComboMultiplier++;
                    SetHorizontalForceComboIncrement(_meterComboMultiplier * 0.1f);
                    SetDropSpeed(levelParametersDataList[_levelIndex].dropSpeed + 0.1f*_meterComboMultiplier);
                    CanLevelUp();
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

                _meterComboMultiplier = 0;
                SetDropSpeed(levelParametersDataList[_levelIndex].dropSpeed);
                SetHorizontalForceComboIncrement(0);
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

        //_meterValue = Mathf.Lerp(_meterValue, _scoreIncrementValue + _meterValue, _meterMoveSpeed * _meterMoveSpeedMultiplier * Time.deltaTime);
        _meterValue += _meterIncrementValue * _meterMoveSpeed * _meterMoveSpeedMultiplier * Time.deltaTime;
        _meterValue = Mathf.Clamp(_meterValue, -1f, 1f);

        if (Mathf.Approximately(waterWaveTransform.position.y, -ScreenBordersCoords.y + 0.8f))
        {
            GameOver();
        }

        if (Mathf.Abs(_meterValue) == 1)
        {
            _meterLimitsTimer += Time.deltaTime;
            if (_meterLimitsTimer > _meterLimitsTimeToDeath)
            {
                GameOver();
            }
        }
        else
        {
            _meterLimitsTimer = 0;
        }

        UpdateWhiteBalanceFilter();
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

    //It passes a thought as an argument, activates it, resets the behaviour component and sets his spawn position using the screen borders as reference
    public void ReSpawnThought(ThoughtBehaviour thought)
    {
        thought.gameObject.SetActive(true);
        int randomIndex = selectRandomIndex(_thoughtsAttributesListSpawnRates);
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

    // Receives a list of probabilities (percentages) and returns a random index of it based on the percentages
    public int selectRandomIndex(List<float> spawnRateList)
    {
        float randomPercentage = UnityEngine.Random.Range(0f, 100f);
        float sum = 0;
        for (int index = 0; index < spawnRateList.Count; index++)
        {
            sum += spawnRateList[index];
            if (sum > randomPercentage)
            {
                return index;
            }
        }
        return 0;
    }

    //sets the meter multipliers and score combos based on increment or decrement passed through 
    public void OnCatchEvent(float value)
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
        if (_meterValue < -0.2f || _meterValue > 0.2f)
        {
            //_whiteBalance.temperature.value += Mathf.Sign(_meterValue) * 4 * Time.deltaTime;
            _whiteBalance.temperature.value = (((Mathf.Sign(_meterValue))*Mathf.Abs(_meterValue - Mathf.Sign(_meterValue) * 0.02f)) * 48) -8*Mathf.Sign(_meterValue);
        }
        else
        {
            _whiteBalance.temperature.value = 0;
        }
    }

    void UpdateUI()
    {
        //Timer UI
        TimeSpan time = TimeSpan.FromSeconds(_timer);
        timerUI.text = time.ToString(@"mm\:ss");

        //Score UI
        scoreUI.text = _scoreValue.ToString();

        //Meter UI
        meterUI.value = _meterValue;
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
    public void CanLevelUp()
    {
        if (_scoreValue == scoreGoalsToLevelUp[_levelIndex] && _levelIndex < levelParametersDataList.Count)
        {
            _levelIndex++;
            SetLevelParameters();
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
        showerParticleSystem.Stop();
        yield return new WaitForSeconds(levelTransitionTimeDuration);
        dayUI.gameObject.SetActive(false);
        showerParticleSystem.Play();
        canSpawn = true;
        _canStartGame = true;
    }

    //Set level parameters (ex: drop speed, meter move speed, spawn ratio, etc)
    public void SetLevelParameters()
    {
        switch (_levelIndex)
        {
            case 0:

                _spawnTimer = 0;
                
                //Sets timer and score related default values
                _canStartGame = false;
                _timer = 0;
                _scoreValue = 0;
                _scoreIncrementCombo = 1;
                scoreUI.text = 0.ToString();

                //Sets start meter speed, multiplier, internal value and UI value
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
                
                _spawnTimer = 0;
                
                //Sets timer and score related default value
                 _canStartGame = false;
                _scoreIncrementCombo = 1;

                //Sets start meter speed, multiplier, internal value and UI value
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
                _spawnTimer = 0;
                
                //Sets timer and score related default value
                _canStartGame = false;
                _scoreIncrementCombo = 1;

                //Sets start meter speed, multiplier, internal value and UI value
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

