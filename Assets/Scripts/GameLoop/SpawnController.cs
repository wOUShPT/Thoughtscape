using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnController : MonoBehaviour
{
    
    [Space(30, order = 0)]
    [Header("Spawn Settings", order = 1)]
    [Space(15, order = 2)]

    [Tooltip("ThoughtPrefab asset in project")]
    public ThoughtController thoughtPrefab;
    private List<ThoughtController> _thoughtsPool;

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
        
        _gameController.levelUpEvent.AddListener(UpdateLevelIndex);

        _gameController.startSpawnEvent.AddListener(SpawnThoughts);

        _gameController.stopSpawnEvent.AddListener(DeSpawnThoughts);

        _currentLevelIndex = 0;

        _canSpawn = false;
        
        //Instantiate thoughts and create a pool based on a pre-established capacity
        _thoughtsPool = new List<ThoughtController>();
        for (int i = 0; i < thoughtsPoolCapacity; i++)
        {
            InstantiateThought();
        }

        _thoughtCurrentSpawnRatesList = new List<float>();
        for (int i = 0; i < thoughtsAttributesList.Count; i++)
        {
            _thoughtCurrentSpawnRatesList.Add(0);
        }

        Physics2D.IgnoreLayerCollision(9,9);
    }

    public void UpdateLevelIndex()
    {
        if (_currentLevelIndex < 7)
        {
            _currentLevelIndex++;
        }
    }

    //Instantiate a new thought on the scene
    private void InstantiateThought()
    {
        ThoughtController thought = Instantiate(thoughtPrefab);
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
        UpdateSpawnRate(_gameController.currentMeterValue, _currentLevelIndex);
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
    private void ReUseThought(ThoughtController thought)
    {
        int randomIndex = GetWeightedRandomIndex();
        while (!_gameController.canStartGame && thoughtsAttributesList[randomIndex].category == "Hope"  )
        {
            randomIndex = GetWeightedRandomIndex();
        }
        thought.currentIndex = randomIndex;
        thought.gameObject.SetActive(true);
        thought.ResetPosition();
        SetHorizontalForce(thought);
        thought.SetThought(thoughtsAttributesList[randomIndex]);
        SetDropSpeed(thought);
    }

    private void DeSpawnThought(ThoughtController thought)
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
        StopCoroutine(Spawner());
    }

    public void SetDropSpeed(float dropSpeedMultiplier)
    {
        foreach (var thought in _thoughtsPool)
        {
            thought.dropSpeedMultiplier = dropSpeedMultiplier;
        }
    }
    
    
    private void SetDropSpeed(ThoughtController thought)
    {
        for (int i = 0; i < _gameController.levelParametersDataList[_currentLevelIndex].thoughtSpawnPropertiesList.Count; i++)
        {
            if (thought.category == _gameController.levelParametersDataList[_currentLevelIndex].thoughtSpawnPropertiesList[i].category)
            {
                thought.dropSpeed = _gameController.levelParametersDataList[_currentLevelIndex].thoughtSpawnPropertiesList[i].dropSpeed;
            }
        }
    }
    

    private void SetHorizontalForce(ThoughtController thought)
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
    private int GetWeightedRandomIndex()
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
    
    private void OnDisable()
    {
        _gameController.levelUpEvent.RemoveListener(UpdateLevelIndex);

        _gameController.startSpawnEvent.RemoveListener(SpawnThoughts);

        _gameController.stopSpawnEvent.RemoveListener(DeSpawnThoughts);
    }
    
}

