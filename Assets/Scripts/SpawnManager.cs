using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public ThoughtBehaviour thoughtPrefab;
    public int thoughtsPoolCapacity;
    private List<ThoughtBehaviour> _thoughtsPool;
    
    private float _randomTimeInterval;
    private float _timer;
    
    void Awake()
    {
        _thoughtsPool = new List<ThoughtBehaviour>();
        for (int i = 0; i < thoughtsPoolCapacity; i++)
        {
            InstantiateThought();
        }
        _timer = 0;
    }
    
    void Update()
    {
        _timer += Time.deltaTime;
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

    void InstantiateThought()
    {
        ThoughtBehaviour thought = Instantiate(thoughtPrefab);
        _thoughtsPool.Add(thought);
        thought.gameObject.SetActive(false);
    }

    void ReSpawnThought(ThoughtBehaviour thought)
    {
        thought.gameObject.SetActive(true);
        thought.ResetBehaviour();
    }

    void GenerateRandomTimeInterval()
    {
        _randomTimeInterval = Random.Range(0.5f, 3f);
    }
    
}


    