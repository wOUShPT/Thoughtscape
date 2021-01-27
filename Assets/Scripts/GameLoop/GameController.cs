using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    private SceneManager _sceneManager;
    private SpawnController _spawnController;
    private AudioManager _audioManager;
    private SaveManager _saveManager;
    public Score scoreData;

    #region Level Progression Related Declaration

    [Space(30, order = 0)]
    [Header("Level Progression Settings", order = 1)]
    [Space(15, order = 2)]

    [Tooltip("List of level related parameters data containers")]
    public List<LevelParametersScriptableObject> levelParametersDataList;

    private List<string> _daysList;
    private int _week;
    private int _dayCounter;
    [Tooltip("List of score goals to level up")]
    public List<int> scoreGoalsToLevelUp;
    private int _scoreGoalEndlessMultiplier;
    private int _levelIndex;
    public float levelTransitionTimeDuration;
    private float _levelTransitionTimer;
    
    #endregion
    

    //-------------------------------------------------------------------------------------------------------------

    #region Current Game Related Parameters Declaration
    
    [Space(30, order = 0)]
    [Header("Current Level Settings", order = 1)]
    [Space(15, order = 2)]
    
    [Tooltip("Meter current move speed")]
    public float meterCurrentMoveSpeed;
    
    private bool _canStartGame;
    [SerializeField]
    private int _scoreValue;
    private float _scoreComboTimer;
    private float _scoreTimeInterval;
    private int _scoreComboMultiplier;
    private float _meterIncrementValue;
    private float _lastMeterIncrementValue;
    private float _scoreIncrementCombo;

    public float currentMeterSpreadValue;
    private float _meterValue;
    private bool _isMovingUp;
    private float _meterComboTimer;
    private float _meterComboMultiplier;
    private float _meterMoveSpeed;
    private float _meterMoveSpeedMultiplier;
    private float _meterLimitsTimer;
    private float _meterLimitsTimeToDeath;
    
    public UnityEvent levelUp;       
    private UnityEvent _startSpawn;    
    private UnityEvent _stopSpawn;     
    
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

    private BackgroundTransition _backgroundTransitionController;

    #endregion
    
    //-------------------------------------------------------------------------------------------------------------
    
    #region UI Related Parameters Declaration
   
    [Space(30, order = 0)]
    [Header("UI Settings", order = 1)]
    [Space(15, order = 2)]

    [Tooltip("UI score text component")] 
    public TextMeshProUGUI scoreUI;

    [Tooltip("UI balance meter slider component")]
    public Slider meterSlider;
    private MeterUI _meterUI;

    [Tooltip("UI day text component")]
    public TextMeshProUGUI dayUI;

    [Tooltip("OptionsMenu component")]
    public OptionsMenu optionsMenu;

    #endregion

    //-------------------------------------------------------------------------------------------------------------

    #region Post Processing Parameters Declaration

    [Space(30, order = 0)]
    [Header("Post Processing, Particle System and Effects", order = 1)]
    [Space(15, order = 2)]
    
    [Tooltip("Post Processing Volume component")]
    public Volume postProcessingVolume;
    private WhiteBalance _whiteBalance;
    private ChromaticAberration _chromaticAberration;
    private int _chromaticAberrationSign;

    public ParticleSystem showerParticleSystem;

    [Tooltip("Vapor fog effect animator component")]
    public Animator vaporEffectAnimator;
    
    [Tooltip("Fade transition prefab animator component")]
    public Animator levelFadeTransitionAnimator;

    [Tooltip("Time duration of the catch thoughts chromatic aberration feedback")]
    public float chromaticAberrationFeedbackEffectTime;
    
    #endregion
    
    private Camera _mainCamera;
    private Vector3 _screenBordersCoords;

    //-------------------------------------------------------------------------------------------------------------
    
    void Awake()
    {
        _sceneManager = FindObjectOfType<SceneManager>();

        _spawnController = FindObjectOfType<SpawnController>();

        _saveManager = FindObjectOfType<SaveManager>();
        
        levelUp = new UnityEvent();

        _startSpawn = new UnityEvent();
        
        _stopSpawn = new UnityEvent();
        
        levelUp.AddListener(_spawnController.UpdateLevel);
        
        _startSpawn.AddListener(_spawnController.SpawnThoughts);
        
        _stopSpawn.AddListener(_spawnController.DeSpawnThoughts);

        _audioManager = FindObjectOfType<AudioManager>();
        
        _mainCamera = FindObjectOfType<Camera>();
        
        _meterUI = meterSlider.gameObject.GetComponent<MeterUI>();

        _backgroundTransitionController = FindObjectOfType<BackgroundTransition>();

        Physics2D.IgnoreLayerCollision(9, 9);
        
        //Initialize the first level
        showerParticleSystem.Stop();
        _levelIndex = 0;
        _scoreGoalEndlessMultiplier = 0;
        _week = 1;
        _dayCounter = 0;
        _daysList = new List<string>();
        SetDaysList();
        SetLevelParameters();
        StartCoroutine(LevelTransition());

        //Get postprocessing filters and set base values
        postProcessingVolume.profile.TryGet(out _whiteBalance);
        postProcessingVolume.profile.TryGet(out _chromaticAberration);
        _whiteBalance.temperature.min = -60;
        _whiteBalance.temperature.max = 60;
        _chromaticAberration.intensity.min = 0f;
        _chromaticAberration.intensity.max = 1f;
        _chromaticAberrationSign = 1;

        //Start Water Update Loop
        _canWaterLow = false;
        _canWaterRise = false;
        _waterLevelDefaultPosition = waterWaveTransform.position;
        StartCoroutine(UpdateWaterLevel());
        
        //Set score and game timer to zero
        _scoreValue = 0;
    }


    private void OnDisable()
    {
        levelUp.RemoveListener(_spawnController.UpdateLevel);        
                                                        
        _startSpawn.RemoveListener(_spawnController.SpawnThoughts);   
                                                        
        _stopSpawn.RemoveListener(_spawnController.DeSpawnThoughts);  
    }

    void Update()
    {
        //Check if the player caught a thought and if it's true begins the timer, score and meter logic
        if (_canStartGame)
        {

            //Update the spawn rate in accord to the meter current position
            _spawnController.UpdateSpawnRate(_meterValue, _levelIndex);

            //Checks if the player it's in the center zone of the meter
            if (_meterValue < currentMeterSpreadValue && _meterValue > -currentMeterSpreadValue)
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
                    CheckIfLevelUp();
                }
                
                if (_meterComboTimer > 5)
                {
                    _meterComboTimer = 0;
                    _meterComboMultiplier += 0.1f;
                    _spawnController.SetDropSpeed(_meterComboMultiplier);
                }
                
                _meterMoveSpeedMultiplier =
                    Mathf.Abs(Mathf.Abs(_meterValue)-(1*currentMeterSpreadValue*1.5f)) * _scoreIncrementCombo;
            }
            else
            {
                _canWaterRise = true;
                _canWaterLow = false;
                _meterComboTimer = 0;
                _scoreComboTimer = 0;
                _scoreComboMultiplier = 0;
                _meterComboMultiplier = 1;
                
                _spawnController.SetDropSpeed(1f);

                if (_isMovingUp && Mathf.Sign(_meterValue) == -1 || !_isMovingUp && Mathf.Sign(_meterValue) == 1)
                {
                    _meterMoveSpeedMultiplier = (Mathf.Abs(_meterValue)/1.5f) * _scoreIncrementCombo;
                }
                else
                {
                    _meterMoveSpeedMultiplier = ((1 - Mathf.Abs(_meterValue))/1.5f) * _scoreIncrementCombo;
                }

                if (Mathf.Abs(_meterValue) == 1)
                {
                    _meterLimitsTimer += Time.deltaTime;
                    if (_meterLimitsTimer > _meterLimitsTimeToDeath)
                    {
                        GameOver();
                    }
                }

                _meterLimitsTimer = 0;
            }
            
            _meterValue += _meterIncrementValue * _meterMoveSpeed * _meterMoveSpeedMultiplier * Time.deltaTime;
            _meterValue = Mathf.Clamp(_meterValue, meterSlider.minValue, meterSlider.maxValue);

            if (Mathf.Approximately(waterWaveTransform.position.y, ScreenProperties.currentScreenCoords.yMin + 0.8f))
            {
                GameOver();
            }
        }

        UpdateWhiteBalanceFilter();

        //UI Update
        UpdateUI();
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
            _canStartGame = false;
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
                _scoreIncrementCombo = Mathf.Clamp(_scoreIncrementCombo + 0.1f, 1, 10);
                _meterIncrementValue = value;
            }
            _lastMeterIncrementValue = value;
        }
        else
        {
            if (value == 10)
            {
                _meterValue = 0;
                _meterMoveSpeedMultiplier = 2;
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
        _spawnController.DeSpawnThoughts();
        scoreData.lastScore = _scoreValue;
        _saveManager.SaveData();
        if (scoreData.bestScore <= scoreData.lastScore)
        {
            scoreData.bestScore = scoreData.lastScore;
        }
        _sceneManager.LoadScene(3);
    }

    //---------------- PostProcessing, Visual Effects and UI Related Functions ------------------------------------

    //Updates post processing white balance filter based on the meter balance values
    void UpdateWhiteBalanceFilter()
    {
        _whiteBalance.temperature.value = _meterValue*_whiteBalance.temperature.max;
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
        //Update Score value on UI
        scoreUI.text = _scoreValue.ToString();
        
        //Update Meter Value on UI
        meterSlider.value = _meterValue;
    }

    //Water level Update loop
    IEnumerator UpdateWaterLevel()
    {
        while (true)
        {
            if (_canWaterLow)
            {
                waterWaveTransform.position = new Vector3(waterWaveTransform.position.x,
                    Mathf.Clamp(waterWaveTransform.position.y - waterLevelDropSpeed * Time.deltaTime,
                        _waterLevelDefaultPosition.y, ScreenProperties.currentScreenCoords.yMin+ 0.8f)
                    , waterWaveTransform.position.z);
                yield return null;
            }

            if (_canWaterRise)
            {
                waterWaveTransform.position = new Vector3(waterWaveTransform.position.x,
                    Mathf.Clamp(waterWaveTransform.position.y + waterLevelRiseSpeed * Time.deltaTime,
                        _waterLevelDefaultPosition.y, ScreenProperties.currentScreenCoords.yMin + 0.8f)
                    , waterWaveTransform.position.z);
            }

            if (!_canStartGame)
            {
                waterWaveTransform.position =                                                                                                   
                    new Vector3(waterWaveTransform.position.x, _waterLevelDefaultPosition.y, waterWaveTransform.position.z);
            }

            yield return null;
        }
    }
    
    //Check if can level up
    public void CheckIfLevelUp()
    {
        if (_scoreValue == (scoreGoalsToLevelUp[_levelIndex]+(100*_scoreGoalEndlessMultiplier)))
        {
            if (_levelIndex < 7)
            {
                _levelIndex++;
            }
            _dayCounter++;
            levelUp.Invoke();
            StartCoroutine(LevelTransition());
        }
    }

    IEnumerator LevelTransition()
    {
        _stopSpawn.Invoke();
        optionsMenu.Hide();
        _canStartGame = false;
        scoreUI.gameObject.SetActive(false);
        
        if (_levelIndex != 0)
        {
            levelFadeTransitionAnimator.SetTrigger("Start");
            yield return new WaitForSeconds(2f);
            _backgroundTransitionController.ChangeBackgroundProps();
            levelFadeTransitionAnimator.SetTrigger("End");
        }
        else
        {
            _backgroundTransitionController.ChangeBackgroundProps();
        }
        showerParticleSystem.Stop();
        _audioManager.StopShowerLoopSFX();
        vaporEffectAnimator.SetBool("CanFade", false);
        
        SetLevelParameters();
        
        if (_dayCounter == 7)
        {
            _dayCounter = 0;
        }
        dayUI.text = _daysList[_dayCounter];
        dayUI.gameObject.SetActive(true);
        
        yield return new WaitForSeconds(levelTransitionTimeDuration);
        
        dayUI.gameObject.SetActive(false);
        scoreUI.gameObject.SetActive(true);
        optionsMenu.Show();
        showerParticleSystem.Play();
        _audioManager.PlayShowerLoopSFX();
        vaporEffectAnimator.SetBool("CanFade", true);

        yield return new WaitForSeconds(2f);
        
        _startSpawn.Invoke();
    }

    //Set level parameters (ex: drop speed, meter move speed, spawn ratio, etc)
    public void SetLevelParameters()
    {
        switch (_levelIndex)
        {
            case 0:

                //Sets timer and score related default values
                _spawnController.SetSpawnTimeInterval(_levelIndex);
                _spawnController.SetDropSpeed(1);
                _scoreComboTimer = 0;
                _scoreComboMultiplier = 0;
                _meterComboMultiplier = 1;
                _scoreTimeInterval = levelParametersDataList[_levelIndex].scoreBaseTime;
                _scoreIncrementCombo = 1;

                //Sets start meter speed, multiplier, internal value and UI value
                currentMeterSpreadValue = levelParametersDataList[_levelIndex].meterCenterSpreadValue;
                _meterUI.SetMeterUI(currentMeterSpreadValue);
                _meterMoveSpeed = levelParametersDataList[_levelIndex].meterBaseMoveSpeed;
                _meterMoveSpeedMultiplier = 0;
                _meterValue = 0;
                _meterIncrementValue = 0;
                _lastMeterIncrementValue = 0;
                _meterLimitsTimer = 0;
                _meterLimitsTimeToDeath = levelParametersDataList[_levelIndex].meterLimitsTimeToDeath;
                waterLevelDropSpeed = levelParametersDataList[_levelIndex].waterLevelDropSpeed;
                waterLevelRiseSpeed = levelParametersDataList[_levelIndex].waterLevelRiseSpeed;
                break;
            
            case 1:

                //Sets timer and score related default value
                _spawnController.SetSpawnTimeInterval(_levelIndex);
                _spawnController.SetDropSpeed(1);
                _scoreComboTimer = 0;
                _scoreComboMultiplier = 0;
                _meterComboMultiplier = 1;
                _scoreTimeInterval = levelParametersDataList[_levelIndex].scoreBaseTime;
                _scoreIncrementCombo = 1;

                //Sets start meter speed, multiplier, internal value and UI value
                currentMeterSpreadValue = levelParametersDataList[_levelIndex].meterCenterSpreadValue;
                _meterUI.SetMeterUI(currentMeterSpreadValue);
                _meterMoveSpeed = levelParametersDataList[_levelIndex].meterBaseMoveSpeed;
                _meterMoveSpeedMultiplier = 0;
                _meterValue = 0.6f;
                _meterIncrementValue = 0;
                _lastMeterIncrementValue = 0;
                _meterLimitsTimer = 0;
                _meterLimitsTimeToDeath = levelParametersDataList[_levelIndex].meterLimitsTimeToDeath;
                waterLevelDropSpeed = levelParametersDataList[_levelIndex].waterLevelDropSpeed;
                waterLevelRiseSpeed = levelParametersDataList[_levelIndex].waterLevelRiseSpeed;
                break;

            case 2:

                //Sets timer and score related default value
                
                _spawnController.SetSpawnTimeInterval(_levelIndex);             
                _spawnController.SetDropSpeed(1);
                _scoreComboTimer = 0;
                _scoreComboMultiplier = 0;
                _meterComboMultiplier = 1;
                _scoreTimeInterval = levelParametersDataList[_levelIndex].scoreBaseTime;
                _scoreIncrementCombo = 1;

                //Sets start meter speed, multiplier, internal value and UI value
                currentMeterSpreadValue = levelParametersDataList[_levelIndex].meterCenterSpreadValue;
                _meterUI.SetMeterUI(currentMeterSpreadValue);
                _meterMoveSpeed = levelParametersDataList[_levelIndex].meterBaseMoveSpeed;
                _meterMoveSpeedMultiplier = 0;
                _meterValue = -0.6f;
                _meterIncrementValue = 0;
                _lastMeterIncrementValue = 0;
                _meterLimitsTimer = 0;
                _meterLimitsTimeToDeath = levelParametersDataList[_levelIndex].meterLimitsTimeToDeath;
                waterLevelDropSpeed = levelParametersDataList[_levelIndex].waterLevelDropSpeed;
                waterLevelRiseSpeed = levelParametersDataList[_levelIndex].waterLevelRiseSpeed;
                break;

            case 3:

                //Sets timer and score related default value
                
                _spawnController.SetSpawnTimeInterval(_levelIndex);             
                _spawnController.SetDropSpeed(1);
                _scoreComboTimer = 0;
                _scoreComboMultiplier = 0;
                _meterComboMultiplier = 1;
                _scoreTimeInterval = levelParametersDataList[_levelIndex].scoreBaseTime;
                _scoreIncrementCombo = 1;

                //Sets start meter speed, multiplier, internal value and UI value
                
                currentMeterSpreadValue = levelParametersDataList[_levelIndex].meterCenterSpreadValue;
                _meterUI.SetMeterUI(currentMeterSpreadValue);
                _meterMoveSpeed = levelParametersDataList[_levelIndex].meterBaseMoveSpeed;
                _meterMoveSpeedMultiplier = 0;
                _meterValue = -0.8f;
                _meterIncrementValue = 0;
                _lastMeterIncrementValue = 0;
                _meterLimitsTimer = 0;
                _meterLimitsTimeToDeath = levelParametersDataList[_levelIndex].meterLimitsTimeToDeath;
                waterLevelDropSpeed = levelParametersDataList[_levelIndex].waterLevelDropSpeed;
                waterLevelRiseSpeed = levelParametersDataList[_levelIndex].waterLevelRiseSpeed;
                break;

            case 4:

                //Sets timer and score related default value
                
                _spawnController.SetSpawnTimeInterval(_levelIndex);             
                _spawnController.SetDropSpeed(1);
                _scoreComboTimer = 0;
                _scoreComboMultiplier = 0;
                _meterComboMultiplier = 1;
                _scoreTimeInterval = levelParametersDataList[_levelIndex].scoreBaseTime;
                _scoreIncrementCombo = 1;

                //Sets start meter speed, multiplier, internal value and UI value
                
                currentMeterSpreadValue = levelParametersDataList[_levelIndex].meterCenterSpreadValue;
                _meterUI.SetMeterUI(currentMeterSpreadValue);
                _meterMoveSpeed = levelParametersDataList[_levelIndex].meterBaseMoveSpeed;
                _meterMoveSpeedMultiplier = 0;
                _meterValue = 0f;
                _meterIncrementValue = 0;
                _lastMeterIncrementValue = 0;
                _meterLimitsTimer = 0;
                _meterLimitsTimeToDeath = levelParametersDataList[_levelIndex].meterLimitsTimeToDeath;
                waterLevelDropSpeed = levelParametersDataList[_levelIndex].waterLevelDropSpeed;
                waterLevelRiseSpeed = levelParametersDataList[_levelIndex].waterLevelRiseSpeed;
                break;
            
            case 5:

                //Sets timer and score related default value
                
                _spawnController.SetSpawnTimeInterval(_levelIndex);             
                _spawnController.SetDropSpeed(1);
                _scoreComboTimer = 0;
                _scoreComboMultiplier = 0;
                _meterComboMultiplier = 1;
                _scoreTimeInterval = levelParametersDataList[_levelIndex].scoreBaseTime;
                _scoreIncrementCombo = 1;

                //Sets start meter speed, multiplier, internal value and UI value
                
                currentMeterSpreadValue = levelParametersDataList[_levelIndex].meterCenterSpreadValue;
                _meterUI.SetMeterUI(currentMeterSpreadValue);
                _meterMoveSpeed = levelParametersDataList[_levelIndex].meterBaseMoveSpeed;
                _meterMoveSpeedMultiplier = 0;
                _meterValue = -0.6f;
                _meterIncrementValue = 0;
                _lastMeterIncrementValue = 0;
                _meterLimitsTimer = 0;
                _meterLimitsTimeToDeath = levelParametersDataList[_levelIndex].meterLimitsTimeToDeath;
                waterLevelDropSpeed = levelParametersDataList[_levelIndex].waterLevelDropSpeed;
                waterLevelRiseSpeed = levelParametersDataList[_levelIndex].waterLevelRiseSpeed;
                break;
            
            case 6:

                _spawnController.SetSpawnTimeInterval(_levelIndex);             
                _spawnController.SetDropSpeed(1);
                _scoreComboTimer = 0;
                _scoreComboMultiplier = 0;
                _meterComboMultiplier = 1;
                _scoreTimeInterval = levelParametersDataList[_levelIndex].scoreBaseTime;
                _scoreIncrementCombo = 1;

                //Sets start meter speed, multiplier, internal value and UI value
                currentMeterSpreadValue = levelParametersDataList[_levelIndex].meterCenterSpreadValue;
                _meterUI.SetMeterUI(currentMeterSpreadValue);
                _meterMoveSpeed = levelParametersDataList[_levelIndex].meterBaseMoveSpeed;
                _meterMoveSpeedMultiplier = 0;
                _meterValue = 0.8f;
                _meterIncrementValue = 0;
                _lastMeterIncrementValue = 0;
                _meterLimitsTimer = 0;
                _meterLimitsTimeToDeath = levelParametersDataList[_levelIndex].meterLimitsTimeToDeath;
                waterLevelDropSpeed = levelParametersDataList[_levelIndex].waterLevelDropSpeed;
                waterLevelRiseSpeed = levelParametersDataList[_levelIndex].waterLevelRiseSpeed;
                break;

            case 7:
                
                _scoreGoalEndlessMultiplier++;
                _spawnController.SetSpawnTimeInterval(_levelIndex);             
                _spawnController.SetDropSpeed(1);
                _scoreComboTimer = 0;
                _scoreComboMultiplier = 0;
                _meterComboMultiplier = 1;
                _scoreTimeInterval = levelParametersDataList[_levelIndex].scoreBaseTime;
                _scoreIncrementCombo = 1;

                //Sets start meter speed, multiplier, internal value and UI value
                currentMeterSpreadValue = levelParametersDataList[_levelIndex].meterCenterSpreadValue;
                _meterUI.SetMeterUI(currentMeterSpreadValue);
                _meterMoveSpeed = levelParametersDataList[_levelIndex].meterBaseMoveSpeed;
                _meterMoveSpeedMultiplier = 0;
                _meterValue = Random.Range(-0.8f,0.8f);
                _meterIncrementValue = 0;
                _lastMeterIncrementValue = 0;
                _meterLimitsTimer = 0;
                _meterLimitsTimeToDeath = levelParametersDataList[_levelIndex].meterLimitsTimeToDeath;
                waterLevelDropSpeed = levelParametersDataList[_levelIndex].waterLevelDropSpeed;
                waterLevelRiseSpeed = levelParametersDataList[_levelIndex].waterLevelRiseSpeed;
                break;
        }
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
}

