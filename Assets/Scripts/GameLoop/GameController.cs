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
    private InputManager _inputManager;
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

    public bool canStartGame;
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
    
    private BackgroundTransitionBehaviour _backgroundTransitionBehaviourController;

    #endregion

    //-------------------------------------------------------------------------------------------------------------

    #region Post Processing and Various Effects Variables Declaration

    [Space(30, order = 0)] [Header("Post Processing, Particle System and Effects", order = 1)] [Space(15, order = 2)]

    private WaterController _waterController;

    [Tooltip("Show particle system component")]
    public ParticleSystem showerParticleSystem;

    [Tooltip("Vapor fog effect animator component")]
    public Animator vaporEffectAnimator;
    
    [Tooltip("Fade transition prefab animator component")]
    public Animator levelFadeTransitionAnimator;
    
    private ChromaticAberrationFeedbackEffect _chromaticAberrationFeedbackEffect;
    
    #endregion
    
    #region Events

    public ScoreEvent scoreEvent;
    public UnityEvent levelUpEvent;       
    public UnityEvent startSpawnEvent;    
    public UnityEvent stopSpawnEvent;

    #endregion

    //-------------------------------------------------------------------------------------------------------------
    
    void Awake()
    {
        _sceneManager = FindObjectOfType<SceneManager>();
        
        _saveManager = FindObjectOfType<SaveManager>();

        _inputManager = FindObjectOfType<InputManager>();

        _spawnController = FindObjectOfType<SpawnController>();

        _uiController = FindObjectOfType<UIController>();

        _audioManager = FindObjectOfType<AudioManager>();

        _backgroundTransitionBehaviourController = FindObjectOfType<BackgroundTransitionBehaviour>();

        _chromaticAberrationFeedbackEffect = FindObjectOfType<ChromaticAberrationFeedbackEffect>();

        _waterController = FindObjectOfType<WaterController>();

        //Initialize the first level
        showerParticleSystem.Stop();
        _levelIndex = 0;
        _scoreGoalEndlessMultiplier = 0;
        SetLevelParameters();
        StartCoroutine(LevelTransition());
        
        //Set score and game timer to zero
        _scoreValue = 0;
        scoreEvent.Invoke(_scoreValue);
    }


    private void OnDisable()
    {
        levelUpEvent.RemoveListener(_spawnController.UpdateLevelIndex);        
                                                        
        startSpawnEvent.RemoveListener(_spawnController.SpawnThoughts);   
                                                        
        stopSpawnEvent.RemoveListener(_spawnController.DeSpawnThoughts);  
    }

    void Update()
    {
        //Check if the player caught a thought and if it's true begins the timer, score and meter logic
        if (canStartGame)
        {
            //Update the spawn rate in accord to the meter current position
            _spawnController.UpdateSpawnRate(currentMeterValue, _levelIndex);

            //Checks if the player it's in the center zone of the meter
            if (currentMeterValue < currentMeterSpreadValue && currentMeterValue > -currentMeterSpreadValue)
            {
                _waterController.canWaterRise = false;
                _waterController.canWaterLow = true;
                _meterComboTimer += Time.deltaTime;
                _scoreComboTimer += Time.deltaTime;

                //Score if exceeds the time to score set
                if (_scoreComboTimer > Mathf.Clamp(_scoreTimeInterval+1 - (_scoreComboMultiplier), 0.2f, _scoreTimeInterval + 1))
                {
                    _scoreComboMultiplier++;
                    _scoreValue += 2;
                    scoreEvent.Invoke(_scoreValue);
                    _scoreComboTimer = 0;
                    _uiController.PopScoreUI();
                    CheckIfLevelUp();
                }
                
                if (_meterComboTimer > 5)
                {
                    _meterComboTimer = 0;
                    _meterComboMultiplier += 0.1f;
                    _spawnController.SetDropSpeed(_meterComboMultiplier);
                }
                
                _meterMoveSpeedMultiplier = Mathf.Abs(Mathf.Abs(currentMeterValue)-(1*currentMeterSpreadValue*1.5f)) * _scoreIncrementCombo;
            }
            else
            {
                _waterController.canWaterRise = true;
                _waterController.canWaterLow = false;
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
            }
            
            currentMeterValue += _meterIncrementValue * _meterMoveSpeed * _meterMoveSpeedMultiplier * Time.deltaTime;
            currentMeterValue = Mathf.Clamp(currentMeterValue, _uiController.meterSlider.minValue, _uiController.meterSlider.maxValue);
            
            if (Mathf.Approximately(_waterController.waterWaveTransform.position.y, ScreenProperties.currentScreenCoords.yMin + 0.8f))
            {
                GameOver();
            }
        }
    }

    private void LateUpdate()
    {
        //UI Update
        _uiController.UpdateMeterUI(currentMeterValue);
    }

    //sets the meter multipliers and score combos based on increment or decrement passed through 
    public void OnCatchEvent(float value)
    {
        _inputManager.Vibrate();
        _chromaticAberrationFeedbackEffect.Feedback();
        _audioManager.PlayCatch();
        if (_lastMeterIncrementValue == 0)
        {
            _uiController.IdleScoreUI(false);
            canStartGame = true;
        }

        if (value == -10)
        {
            canStartGame = false;
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
        canStartGame = false;
        stopSpawnEvent.Invoke();
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

    //Check if can level up
    private void CheckIfLevelUp()
    {
        if (_scoreValue != (scoreGoalsToLevelUp[_levelIndex]+(100*_scoreGoalEndlessMultiplier)))
        {
            return;
        }
        if (_levelIndex < 7)
        {
            _levelIndex++;
        }
        levelUpEvent.Invoke();
        StartCoroutine(LevelTransition());
    }

    IEnumerator LevelTransition()
    {
        stopSpawnEvent.Invoke();
        _uiController.ShowOptionMenu(false);
        canStartGame = false;
        _uiController.ShowScoreUI(false);
        
        if (_levelIndex != 0)
        {
            levelFadeTransitionAnimator.SetTrigger("Start");
            GC.Collect();
            yield return new WaitForSeconds(2f);
            _waterController.ResetWaterLevel();
            _backgroundTransitionBehaviourController.ChangeBackgroundProps();
            levelFadeTransitionAnimator.SetTrigger("End");
        }
        else
        {
            _backgroundTransitionBehaviourController.ChangeBackgroundProps();
        }
        showerParticleSystem.Stop();
        _audioManager.StopShowerLoopSFX();
        vaporEffectAnimator.SetBool("CanFade", false);
        
        SetLevelParameters();

        _uiController.UpdateDayUI();
        _uiController.ShowDayUI(true);

        yield return new WaitForSeconds(levelTransitionTimeDuration);
        
        _uiController.ShowDayUI(false);
        _uiController.ShowScoreUI(true);
        _uiController.IdleScoreUI(true);
        _uiController.ShowOptionMenu(true);
        showerParticleSystem.Play();
        _audioManager.PlayShowerLoopSFX();
        vaporEffectAnimator.SetBool("CanFade", true);

        yield return new WaitForSeconds(2f);

        startSpawnEvent.Invoke();
    }

    //Set level parameters (ex: drop speed, meter move speed, spawn ratio, etc)
    private void SetLevelParameters()
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
                _uiController.SetMeterUI(currentMeterSpreadValue);
                _meterMoveSpeed = levelParametersDataList[_levelIndex].meterBaseMoveSpeed;
                _meterMoveSpeedMultiplier = 0;
                currentMeterValue = 0;
                _meterIncrementValue = 0;
                _lastMeterIncrementValue = 0; ;
                _waterController.waterLevelDropSpeed = levelParametersDataList[_levelIndex].waterLevelDropSpeed;
                _waterController.waterLevelRiseSpeed = levelParametersDataList[_levelIndex].waterLevelRiseSpeed;
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
                _uiController.SetMeterUI(currentMeterSpreadValue);
                _meterMoveSpeed = levelParametersDataList[_levelIndex].meterBaseMoveSpeed;
                _meterMoveSpeedMultiplier = 0;
                currentMeterValue = 0.6f;
                _meterIncrementValue = 0;
                _lastMeterIncrementValue = 0;
                _waterController.waterLevelDropSpeed = levelParametersDataList[_levelIndex].waterLevelDropSpeed;
                _waterController.waterLevelRiseSpeed = levelParametersDataList[_levelIndex].waterLevelRiseSpeed;
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
                _uiController.SetMeterUI(currentMeterSpreadValue);
                _meterMoveSpeed = levelParametersDataList[_levelIndex].meterBaseMoveSpeed;
                _meterMoveSpeedMultiplier = 0;
                currentMeterValue = -0.6f;
                _meterIncrementValue = 0;
                _lastMeterIncrementValue = 0;
                _waterController.waterLevelDropSpeed = levelParametersDataList[_levelIndex].waterLevelDropSpeed;
                _waterController.waterLevelRiseSpeed = levelParametersDataList[_levelIndex].waterLevelRiseSpeed;
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
                _uiController.SetMeterUI(currentMeterSpreadValue);
                _meterMoveSpeed = levelParametersDataList[_levelIndex].meterBaseMoveSpeed;
                _meterMoveSpeedMultiplier = 0;
                currentMeterValue = -0.8f;
                _meterIncrementValue = 0;
                _lastMeterIncrementValue = 0;
                _waterController.waterLevelDropSpeed = levelParametersDataList[_levelIndex].waterLevelDropSpeed;
                _waterController.waterLevelRiseSpeed = levelParametersDataList[_levelIndex].waterLevelRiseSpeed;
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
                _uiController.SetMeterUI(currentMeterSpreadValue);
                _meterMoveSpeed = levelParametersDataList[_levelIndex].meterBaseMoveSpeed;
                _meterMoveSpeedMultiplier = 0;
                currentMeterValue = 0f;
                _meterIncrementValue = 0;
                _lastMeterIncrementValue = 0;
                _waterController.waterLevelDropSpeed = levelParametersDataList[_levelIndex].waterLevelDropSpeed;
                _waterController.waterLevelRiseSpeed = levelParametersDataList[_levelIndex].waterLevelRiseSpeed;
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
                _uiController.SetMeterUI(currentMeterSpreadValue);
                _meterMoveSpeed = levelParametersDataList[_levelIndex].meterBaseMoveSpeed;
                _meterMoveSpeedMultiplier = 0;
                currentMeterValue = -0.6f;
                _meterIncrementValue = 0;
                _lastMeterIncrementValue = 0;
                _waterController.waterLevelDropSpeed = levelParametersDataList[_levelIndex].waterLevelDropSpeed;
                _waterController.waterLevelRiseSpeed = levelParametersDataList[_levelIndex].waterLevelRiseSpeed;
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
                _uiController.SetMeterUI(currentMeterSpreadValue);
                _meterMoveSpeed = levelParametersDataList[_levelIndex].meterBaseMoveSpeed;
                _meterMoveSpeedMultiplier = 0;
                currentMeterValue = 0.8f;
                _meterIncrementValue = 0;
                _lastMeterIncrementValue = 0;
                _waterController.waterLevelDropSpeed = levelParametersDataList[_levelIndex].waterLevelDropSpeed;
                _waterController.waterLevelRiseSpeed = levelParametersDataList[_levelIndex].waterLevelRiseSpeed;
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
                _uiController.SetMeterUI(currentMeterSpreadValue);
                _meterMoveSpeed = levelParametersDataList[_levelIndex].meterBaseMoveSpeed;
                _meterMoveSpeedMultiplier = 0;
                currentMeterValue = Random.Range(-0.8f,0.8f);
                _meterIncrementValue = 0;
                _lastMeterIncrementValue = 0;
                _waterController.waterLevelDropSpeed = levelParametersDataList[_levelIndex].waterLevelDropSpeed;
                _waterController.waterLevelRiseSpeed = levelParametersDataList[_levelIndex].waterLevelRiseSpeed;
                break;
        }
    }

    [System.Serializable]
    public class ScoreEvent : UnityEvent<int>
    {
        
    }
}

