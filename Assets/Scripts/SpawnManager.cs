using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnManager : MonoBehaviour
{
    [Space(30, order = 0)]
    [Header("Thought Prefab Asset and object pool capacity", order = 1)]
    [Space(15, order = 2)]
    
    [Tooltip("ThoughtPrefab asset in project")]
    public ThoughtBehaviour thoughtPrefab;
    
    [Tooltip("Thoughts object pool maximum capacity (InstantiatedOnLoad)")]
    
    public int thoughtsPoolCapacity;

    [Space(30, order = 0)] 
    [Header("Spawn Settings", order = 1)] 
    [Space(15, order = 2)]

    [Tooltip("Minimum time between spawns in seconds")]
    public float minTimeBetweenSpawns;
    [Tooltip("Maximum time between spawns in seconds")]
    public float maxTimeBetweenSpawns;
    
    private List<ThoughtBehaviour> _thoughtsPool;

    private float _randomTimeInterval;
    private float _timer;
    
    private Camera _mainCamera;
    private Vector3 _screenBordersCoords;
    
    
    void Awake()
    {
        
        _mainCamera = FindObjectOfType<Camera>();
        
        //Get screen size width and height in pixels and convert to world units
        _screenBordersCoords = _mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        
        //Instantiate thoughts and create a pool based on a pre-established capacity
        _thoughtsPool = new List<ThoughtBehaviour>();
        for (int i = 0; i < thoughtsPoolCapacity; i++)
        {
            InstantiateThought();
        }
        
        //Set the drop combo velocity increment
        SetDropSpeed(0);
        
        //ResetSpawnTimer;
        _timer = 0;
        
        //Sets the "Managers" gameobject on hierarchy as a parent (this matters if you load the game from _preload scene)
        if (GameObject.FindGameObjectWithTag("Managers"))
        {
            transform.SetParent(GameObject.FindGameObjectWithTag("Managers").transform);
        }
    }
    
    void Update()
    {
        
        //Increment the timer
        _timer += Time.deltaTime;
        
        //when the timer surpasses a random generated time frame, search through the pool for an available thought to reuse and spawn it
        if (_timer > _randomTimeInterval)
        {
            GenerateRandomTimeInterval();
            _timer = 0;
            for (int i = 0; i < _thoughtsPool.Count; i++)
            {
                if (!_thoughtsPool[i].gameObject.activeSelf)
                {
                    ReSpawnThought(_thoughtsPool[i]);
                    return;
                }
            }
        }
    }

    //Instantiate a new thought on the scene
    void InstantiateThought()
    {
        ThoughtBehaviour thought = Instantiate(thoughtPrefab);
        _thoughtsPool.Add(thought);
        thought.gameObject.SetActive(false);
    }

    //It passes a thought as an argument, activates it, resets the behaviour component and sets his spawn position using the screen borders as reference
    void ReSpawnThought(ThoughtBehaviour thought)
    {
        thought.gameObject.SetActive(true);
        thought.ResetBehaviour();
        thought.transform.position = new Vector3(Random.Range(-_screenBordersCoords.x+0.5f, _screenBordersCoords.x-0.5f),_screenBordersCoords.y+2,0);
    }

    //Generates a random time frame between two given values
    void GenerateRandomTimeInterval()
    {
        _randomTimeInterval = Random.Range(minTimeBetweenSpawns, maxTimeBetweenSpawns);
    }

    public void SetDropSpeed(int dropSpeedIncrementValue)
    {
        foreach (var thought in _thoughtsPool)
        {
            thought.SpeedComboIncrement = dropSpeedIncrementValue * 0.3f;
        }   
    }
}


    