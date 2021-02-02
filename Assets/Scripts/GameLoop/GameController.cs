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
    private UIController _uiController;
    private AudioManager _audioManager;
    private SaveManager _saveManager;
    public ScoreScriptableObject scoreScriptableObjectData;

    #region Level Progression Related Declaration

    [Space(30, order = 0)]
    [Header("Level Progression Settings", order = 1)]
    [Space(15, order = 2)]

    [Tooltip("List of level related parameters data containers")]
    public List<LevelParametersScriptableObject> levelParametersDataList;

    private List<string> _daysList;
    private int _dayCounter;
    [Tooltip("List of score goals to level up")]
    public List<int> scoreGoalsToLevelUp;
    private int _scoreGoalEndlessMultiplier;
    private int _levelIndex;
    public float levelTransitionTimeDuration;
    private float _levelTransitionTimer;
    
    #endregion
    

    //-------------------------------------------------------------------------------------------------------------

    #region Current Game Related Variables Declaration
    
    [Space(30, order = 0)]
    [Header("Current Level Settings", order = 1)]
    [Space(15, order = 2)]
    
    [Tooltip("Meter current move speed")]
    public float meterCurrentMoveSpeed;
    
    private bool _canStartGame;
    private int _scoreValue;
    private float _scoreComboTimer;
    private float _scoreTimeInterval;
    private int _scoreComboMultiplier;
    private float _meterIncrementValue;
    private float _lastMeterIncrementValue;
    private float _scoreIncrementCombo;

    public float currentMeterSpreadValue;
    public float currentMeterValue;
    private bool _isMovingUp;
    private float _meterComboTimer;
    private float _meterComboMultiplier;
    private float _meterMoveSpeed;
    private float _meterMoveSpeedMultiplier;
    private float _meterLimitsTimer;
    private float _meterLimitsTimeToDeath;

    [Space(15, order = 0)]
    [Tooltip("Transform component of water wave gameObject")]
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
    
    #region UI Related Variables Declaration
   
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

    #endregion

    //-------------------------------------------------------------------------------------------------------------

    #region Post Processing and Various Effects Variables Declaration

    [Space(30, order = 0)]
    [Header("Post Processing, Particle System and Effects", order = 1)]
    [Space(15, order = 2)]
    
    [Tooltip("Post Processing Volume component")]
    public Volume postProcessingVolume;
    private WhiteBalance _whiteBalance;
    private ChromaticAberration _chromaticAberration;
    private int _chromaticAberrationSign;

    [Tooltip("Show particle system component")]
    public ParticleSystem showerParticleSystem;

    [Tooltip("Vapor fog effect animator component")]
    public Animator vaporEffectAnimator;
    
    [Tooltip("Fade transition prefab animator component")]
    public Animator levelFadeTransitionAnimator;

    [Tooltip("Time duration of the catch thoughts chromatic aberration feedback")]
    public float chromaticAberrationFeedbackEffectTime;
    
    #endregion
    
    #region Events

    public ScoreEvent scoreEvent;
    public UnityEvent levelUp;       
    public UnityEvent startSpawn;    
    public UnityEvent stopSpawn;
    public UnityEvent startLevel;
    public UnityEvent stopLevel;

    #endregion
    
    private Camera _mainCamera;
    private Vector3 _screenBordersCoords;

    //-------------------------------------------------------------------------------------------------------------
    
    void Awake()
    {
        _sceneManager = FindObjectOfType<SceneManager>();
        
        _saveManager = FindObjectOfType<SaveManager>();

        _spawnController = FindObjectOfType<SpawnController>();

        _uiController = FindObjectOfType<UIController>();

        _audioManager = FindObjectOfType<AudioManager>();
        
        _mainCamera = FindObjectOfType<Camera>();
        
        _meterUI = meterSlider.gameObject.GetComponent<MeterUI>();

        _scoreUIAnimator = scoreUI.GetComponent<Animator>();

        _backgroundTransitionController = FindObjectOfType<BackgroundTransition>();

        //Initialize the first level
        showerParticleSystem.Stop();
        _levelIndex = 0;
        _scoreGoalEndlessMultiplier = 0;
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
        scoreEvent.Invoke(_scoreValue);
    }


    private void OnDisable()
    {
        levelUp.RemoveListener(_spawnController.UpdateLevelIndex);        
                                                        
        startSpawn.RemoveListener(_spawnController.SpawnThoughts);   
                                                        
        stopSpawn.RemoveListener(_spawnController.DeSpawnThoughts);  
    }

    void Update()
    {
        //Check if the player caught a thought and if it's true begins the timer, score and meter logic
        if (_canStartGame)
        {
            //Update the spawn rate in accord to the meter current position
            _spawnController.UpdateSpawnRate(currentMeterValue, _levelIndex);

            //Checks if the player it's in the center zone of the meter
            if (currentMeterValue < currentMeterSpreadValue && currentMeterValue > -currentMeterSpreadValue)
            {
                _canWaterRise = false;
                _canWaterLow = true;
                _meterComboTimer += Time.deltaTime;
                _scoreComboTimer += Time.deltaTime;

                if (_scoreComboTimer > Mathf.Clamp(_scoreTimeInterval+1 - (_scoreComboMultiplier), 0.2f, _scoreTimeInterval + 1))
                {
                    _scoreComboMultiplier++;
                    _scoreValue += 2;
                    scoreEvent.Invoke(_scoreValue);
                    _scoreComboTimer = 0;
                    _scoreUIAnimator.SetTrigger("Pop");
                    CheckIfLevelUp();
                }
                
                if (_meterComboTimer > 5)
                {
                    _meterComboTimer = 0;
                    _meterComboMultiplier += 0.1f;
                    _spawnController.SetDropSpeed(_meterComboMultiplier);
                }
                
                _meterMoveSpeedMultiplier =
                    Mathf.Abs(Mathf.Abs(currentMeterValue)-(1*currentMeterSpreadValue*1.5f)) * _scoreIncrementCombo;
            }
            else
            {
                _canWaterRise = true;
                _canWaterLow = false;
                _meterComboTimer = 0;
                _scoreComboTimer = 0;
                _scoreComboMultiplier = 1;
                _meterComboMultiplier = 1;
                
                _spawnController.SetDropSpeed(1f);

                if (_isMovingUp && Mathf.Sign(currentMeterValue) == -1 || !_isMovingUp && Mathf.Sign(currentMeterValue) == 1)
                {
                    _meterMoveSpeedMultiplier = (Mathf.Abs(currentMeterValue)/1.5f) * _scoreIncrementCombo;
                }
                else
                {
                    _meterMoveSpeedMultiplier = ((1 - Mathf.Abs(currentMeterValue))/1.5f) * _scoreIncrementCombo;
                }

                if (Mathf.Abs(currentMeterValue) == 1)
                {
                    _meterLimitsTimer += Time.deltaTime;
                    if (_meterLimitsTimer > _meterLimitsTimeToDeath)
                    {
                        GameOver();
                    }
                }

                _meterLimitsTimer = 0;
            }
            
            currentMeterValue += _meterIncrementValue * _meterMoveSpeed * _meterMoveSpeedMultiplier * Time.deltaTime;
            currentMeterValue = Mathf.Clamp(currentMeterValue, meterSlider.minValue, meterSlider.maxValue);

            if (Mathf.Approximately(waterWaveTransform.position.y, ScreenProperties.currentScreenCoords.yMin + 0.8f))
            {
                GameOver();
            }
        }

        UpdateWhiteBalanceFilter();

        //UI Update
        _uiController.UpdateMeterUI(currentMeterValue);
    }
    
    //sets the meter multipliers and score combos based on increment or decrement passed through 
    public void OnCatchEvent(float value)
    {
        StartCoroutine(ChromaticAberrationFeedback());
        _audioManager.PlayCatch();
        if (_lastMeterIncrementValue == 0)
        {
            _scoreUIAnimator.SetBool("Idle", false);
            startLevel.Invoke();
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
                currentMeterValue = 0;
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
        scoreScriptableObjectData.lastScore = _scoreValue;
        if (scoreScriptableObjectData.bestScore <= scoreScriptableObjectData.lastScore)
        {
            scoreScriptableObjectData.bestScore = scoreScriptableObjectData.lastScore;
        }
        _saveManager.SaveData();
        _sceneManager.LoadScene(3);
    }

    //---------------- PostProcessing, Visual Effects and UI Related Functions ------------------------------------

    //Updates post processing white balance filter based on the meter balance values
    void UpdateWhiteBalanceFilter()
    {
        _whiteBalance.temperature.value = currentMeterValue*_whiteBalance.temperature.max;
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
    
    public IEnumerator LensesDistortionEffect()
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
        meterSlider.value = currentMeterValue;
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
        stopSpawn.Invoke();
        optionsMenu.Hide();
        _canStartGame = false;
        stopLevel.Invoke();
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
        
        _uiController.UpdateDayUI();

        yield return new WaitForSeconds(levelTransitionTimeDuration);
        
        dayUI.gameObject.SetActive(false);
        scoreUI.gameObject.SetActive(true);
        _scoreUIAnimator.SetBool("Idle", true);
        optionsMenu.Show();
        showerParticleSystem.Play();
        _audioManager.PlayShowerLoopSFX();
        vaporEffectAnimator.SetBool("CanFade", true);

        yield return new WaitForSeconds(2f);

        startSpawn.Invoke();
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
                _scoreComboMultiplier = 1;
                _meterComboMultiplier = 1;
                _scoreTimeInterval = levelParametersDataList[_levelIndex].scoreBaseTime;
                _scoreIncrementCombo = 1;

                //Sets start meter speed, multiplier, internal value and UI value
                currentMeterSpreadValue = levelParametersDataList[_levelIndex].meterCenterSpreadValue;
                _meterUI.SetMeterUI(currentMeterSpreadValue);
                _meterMoveSpeed = levelParametersDataList[_levelIndex].meterBaseMoveSpeed;
                _meterMoveSpeedMultiplier = 0;
                currentMeterValue = 0;
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
                _scoreComboMultiplier = 1;
                _meterComboMultiplier = 1;
                _scoreTimeInterval = levelParametersDataList[_levelIndex].scoreBaseTime;
                _scoreIncrementCombo = 1;

                //Sets start meter speed, multiplier, internal value and UI value
                currentMeterSpreadValue = levelParametersDataList[_levelIndex].meterCenterSpreadValue;
                _meterUI.SetMeterUI(currentMeterSpreadValue);
                _meterMoveSpeed = levelParametersDataList[_levelIndex].meterBaseMoveSpeed;
                _meterMoveSpeedMultiplier = 0;
                currentMeterValue = 0.6f;
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
                _scoreComboMultiplier = 1;
                _meterComboMultiplier = 1;
                _scoreTimeInterval = levelParametersDataList[_levelIndex].scoreBaseTime;
                _scoreIncrementCombo = 1;

                //Sets start meter speed, multiplier, internal value and UI value
                currentMeterSpreadValue = levelParametersDataList[_levelIndex].meterCenterSpreadValue;
                _meterUI.SetMeterUI(currentMeterSpreadValue);
                _meterMoveSpeed = levelParametersDataList[_levelIndex].meterBaseMoveSpeed;
                _meterMoveSpeedMultiplier = 0;
                currentMeterValue = -0.6f;
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
                currentMeterValue = -0.8f;
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
                _scoreComboMultiplier = 1;
                _meterComboMultiplier = 1;
                _scoreTimeInterval = levelParametersDataList[_levelIndex].scoreBaseTime;
                _scoreIncrementCombo = 1;

                //Sets start meter speed, multiplier, internal value and UI value
                currentMeterSpreadValue = levelParametersDataList[_levelIndex].meterCenterSpreadValue;
                _meterUI.SetMeterUI(currentMeterSpreadValue);
                _meterMoveSpeed = levelParametersDataList[_levelIndex].meterBaseMoveSpeed;
                _meterMoveSpeedMultiplier = 0;
                currentMeterValue = 0f;
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
                _scoreComboMultiplier = 1;
                _meterComboMultiplier = 1;
                _scoreTimeInterval = levelParametersDataList[_levelIndex].scoreBaseTime;
                _scoreIncrementCombo = 1;

                //Sets start meter speed, multiplier, internal value and UI value
                currentMeterSpreadValue = levelParametersDataList[_levelIndex].meterCenterSpreadValue;
                _meterUI.SetMeterUI(currentMeterSpreadValue);
                _meterMoveSpeed = levelParametersDataList[_levelIndex].meterBaseMoveSpeed;
                _meterMoveSpeedMultiplier = 0;
                currentMeterValue = -0.6f;
                _meterIncrementValue = 0;
                _lastMeterIncrementValue = 0;
                _meterLimitsTimer = 0;
                _meterLimitsTimeToDeath = levelParametersDataList[_levelIndex].meterLimitsTimeToDeath;
                waterLevelDropSpeed = levelParametersDataList[_levelIndex].waterLevelDropSpeed;
                waterLevelRiseSpeed = levelParametersDataList[_levelIndex].waterLevelRiseSpeed;
                break;
            
            case 6:

                //Sets timer and score related default value
                _spawnController.SetSpawnTimeInterval(_levelIndex);             
                _spawnController.SetDropSpeed(1);
                _scoreComboTimer = 0;
                _scoreComboMultiplier = 1;
                _meterComboMultiplier = 1;
                _scoreTimeInterval = levelParametersDataList[_levelIndex].scoreBaseTime;
                _scoreIncrementCombo = 1;

                //Sets start meter speed, multiplier, internal value and UI value
                currentMeterSpreadValue = levelParametersDataList[_levelIndex].meterCenterSpreadValue;
                _meterUI.SetMeterUI(currentMeterSpreadValue);
                _meterMoveSpeed = levelParametersDataList[_levelIndex].meterBaseMoveSpeed;
                _meterMoveSpeedMultiplier = 0;
                currentMeterValue = 0.8f;
                _meterIncrementValue = 0;
                _lastMeterIncrementValue = 0;
                _meterLimitsTimer = 0;
                _meterLimitsTimeToDeath = levelParametersDataList[_levelIndex].meterLimitsTimeToDeath;
                waterLevelDropSpeed = levelParametersDataList[_levelIndex].waterLevelDropSpeed;
                waterLevelRiseSpeed = levelParametersDataList[_levelIndex].waterLevelRiseSpeed;
                break;

            case 7:
                
                //Sets timer and score related default value
                _scoreGoalEndlessMultiplier++;
                _spawnController.SetSpawnTimeInterval(_levelIndex);             
                _spawnController.SetDropSpeed(1);
                _scoreComboTimer = 0;
                _scoreComboMultiplier = 1;
                _meterComboMultiplier = 1;
                _scoreTimeInterval = levelParametersDataList[_levelIndex].scoreBaseTime;
                _scoreIncrementCombo = 1;

                //Sets start meter speed, multiplier, internal value and UI value
                currentMeterSpreadValue = levelParametersDataList[_levelIndex].meterCenterSpreadValue;
                _meterUI.SetMeterUI(currentMeterSpreadValue);
                _meterMoveSpeed = levelParametersDataList[_levelIndex].meterBaseMoveSpeed;
                _meterMoveSpeedMultiplier = 0;
                currentMeterValue = Random.Range(-0.8f,0.8f);
                _meterIncrementValue = 0;
                _lastMeterIncrementValue = 0;
                _meterLimitsTimer = 0;
                _meterLimitsTimeToDeath = levelParametersDataList[_levelIndex].meterLimitsTimeToDeath;
                waterLevelDropSpeed = levelParametersDataList[_levelIndex].waterLevelDropSpeed;
                waterLevelRiseSpeed = levelParametersDataList[_levelIndex].waterLevelRiseSpeed;
                break;
        }
    }

    [System.Serializable]
    public class ScoreEvent : UnityEvent<int>
    {
        
    }
}

