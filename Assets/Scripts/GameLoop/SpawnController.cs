using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnController : MonoBehaviour
{
    
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

    [Tooltip("Center zone spread spawn change tolerance")]
    public float centerZoneSpreadSpawnTolerance;

    [Tooltip("Minimum time between spawns in seconds")]
    public float currentMinTimeBetweenSpawns;
    [Tooltip("Maximum time between spawns in seconds")]
    public float currentMaxTimeBetweenSpawns;

    private int _currentLevelIndex;
    private bool _canSpawn;
    
    private GameController _gameController;
    private float _randomTimeInterval;
    void Awake()
    {
        _gameController = FindObjectOfType<GameController>();

        _currentLevelIndex = 0;

        _canSpawn = false;
        
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
    }

    public void UpdateLevel()
    {
        if (_currentLevelIndex < 7)
        {
            _currentLevelIndex++;
        }
    }

    //Instantiate a new thought on the scene
    public void InstantiateThought()
    {
        ThoughtBehaviour thought = Instantiate(thoughtPrefab);
        thought.gameObject.SetActive(false);
        _thoughtsPool.Add(thought);
    }

    public void SpawnThoughts()
    {
        _canSpawn = true;
        StartCoroutine(Spawner());
    }
    
    IEnumerator Spawner()
    {
        while (_canSpawn)
        {
            for (int i = 0; i < _thoughtsPool.Count; i++)
            {
                if (!_thoughtsPool[i].gameObject.activeSelf)
                {
                    ReUseThought(_thoughtsPool[i]);
                    break;
                }
            }
            _randomTimeInterval = Random.Range(currentMinTimeBetweenSpawns, currentMaxTimeBetweenSpawns);
            yield return new WaitForSeconds(_randomTimeInterval);
            yield return null;
        }
    }

    //It passes a thought as an argument, activates it, resets the behaviour component and sets his spawn position using the screen borders as reference
    public void ReUseThought(ThoughtBehaviour thought)
    {
        int randomIndex = GetWeightedRandomIndex();
        thought.currentIndex = randomIndex;
        thought.gameObject.SetActive(true);
        thought.ResetBehaviour();
        SetDropSpeed(thought);
        SetHorizontalForce(thought);
    }

    private void DeSpawnThought(ThoughtBehaviour thought)
    {
        if (thought.isActiveAndEnabled)
        {
            thought.Fade();
        }
    }

    public void DeSpawnThoughts()
    {
        _canSpawn = false;
        foreach (var thought in _thoughtsPool)
        {
            DeSpawnThought(thought);
        }
    }
    
    public void SetDropSpeed(float dropSpeedMultiplier)
    {
        foreach (var thought in _thoughtsPool)
        {
            thought.dropSpeedMultiplier = dropSpeedMultiplier;
        }
    }
    
    
    public void SetDropSpeed(ThoughtBehaviour thought)
    {
        for (int i = 0; i < _gameController.levelParametersDataList[_currentLevelIndex].thoughtSpawnPropertiesList.Count; i++)
        {
            if (thought.category == _gameController.levelParametersDataList[_currentLevelIndex].thoughtSpawnPropertiesList[i].category)
            {
                thought.dropSpeed = _gameController.levelParametersDataList[_currentLevelIndex].thoughtSpawnPropertiesList[i].dropSpeed;
            }
        }
    }
    

    public void SetHorizontalForce(ThoughtBehaviour thought)
    {
        for (int i = 0; i < _gameController.levelParametersDataList[_currentLevelIndex].thoughtSpawnPropertiesList.Count; i++)
        {
            if (thought.category == _gameController.levelParametersDataList[_currentLevelIndex].thoughtSpawnPropertiesList[i].category)
            {
                thought.hasHorizontalMovement = _gameController.levelParametersDataList[_currentLevelIndex].thoughtSpawnPropertiesList[i]
                    .hasHorizontalForce;
            }
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

    public void UpdateSpawnRate(float meterValue, int currentLevelIndex)
    {
        if (meterValue < _gameController.currentMeterSpreadValue && meterValue > -_gameController.currentMeterSpreadValue)
        {
            for (int i = 0; i < _gameController.levelParametersDataList[currentLevelIndex].thoughtSpawnPropertiesList.Count; i++)
            {
                _thoughtCurrentSpawnRatesList[i] = _gameController.levelParametersDataList[currentLevelIndex]
                    .thoughtSpawnPropertiesList[i].centerMeterZoneSpawnRatePercentage;
            }
        }

        if (meterValue <= -_gameController.currentMeterSpreadValue - centerZoneSpreadSpawnTolerance)
        {
            for (int i = 0; i < _gameController.levelParametersDataList[currentLevelIndex].thoughtSpawnPropertiesList.Count; i++)
            {
                _thoughtCurrentSpawnRatesList[i] = _gameController.levelParametersDataList[currentLevelIndex]
                    .thoughtSpawnPropertiesList[i].negativeZoneMeterSpawnRatePercentage;
            }
        }
                
        if(meterValue >= _gameController.currentMeterSpreadValue + centerZoneSpreadSpawnTolerance)
        {
            for (int i = 0; i < _gameController.levelParametersDataList[currentLevelIndex].thoughtSpawnPropertiesList.Count; i++)
            {
                _thoughtCurrentSpawnRatesList[i] = _gameController.levelParametersDataList[currentLevelIndex]
                    .thoughtSpawnPropertiesList[i].positiveZoneMeterSpawnRatePercentage;
            }
        }
    }

    public void SetSpawnTimeInterval(int currentLevelIndex)
    {
        currentMinTimeBetweenSpawns = _gameController.levelParametersDataList[currentLevelIndex].minTimeBetweenSpawns;            
        currentMaxTimeBetweenSpawns = _gameController.levelParametersDataList[currentLevelIndex].maxTimeBetweenSpawns;            
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
            
            randomWeight = Random.Range(0, sum);
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
}
