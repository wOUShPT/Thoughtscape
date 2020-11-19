using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelsManager : MonoBehaviour
{
    private GameManager _gameManager;
    private int _levelNumber;
    private LevelUpEvent _levelUp;
    public List<int> scoresToLevelUp;
    void Awake()
    {
        _levelUp = new LevelUpEvent();
        _levelUp.AddListener(_gameManager.SetValues());
        _levelNumber = 0;
    }

    public void LevelUp(int score)
    {
        if (score == scoresToLevelUp[_levelNumber])
        {
            
            _levelUp.Invoke(_levelNumber);
            _levelNumber++;
        }
    }

    public class LevelUpEvent: UnityEvent<int>
    {
        
    }
}
