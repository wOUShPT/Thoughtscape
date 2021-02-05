using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Slider = UnityEngine.UI.Slider;

public class BackgroundTransitionBehaviour : MonoBehaviour
{
    private GameController _gameController;
    public float transitionSpeed;
    public SpriteRenderer neutralWallSprite;
    public SpriteRenderer positiveWallSprite;
    public SpriteRenderer negativeWallSprite;
    public SpriteRenderer neutralShelfSprite;
    public SpriteRenderer positiveShelfSprite;
    public SpriteRenderer negativeShelfSprite;
    public SpriteRenderer positiveSmiles;
    public SpriteRenderer negativeCracks;
    public List<GameObject> backgroundPropsPrefabsList;
    private GameObject _currentProp;
    private bool _isPositive;
    private bool _isNegative;

    void Awake()
    {
        _gameController = FindObjectOfType<GameController>();
        _isNegative = false;
        _isPositive = false;
        neutralWallSprite.material.SetFloat("_DissolveCutoff", 0);
        positiveWallSprite.material.SetFloat("_DissolveCutoff", 1);
        negativeWallSprite.material.SetFloat("_DissolveCutoff", 1);
        neutralShelfSprite.material.SetFloat("_DissolveCutoff", 0);
        positiveShelfSprite.material.SetFloat("_DissolveCutoff", 1);
        negativeShelfSprite.material.SetFloat("_DissolveCutoff", 1);
        positiveSmiles.material.SetFloat("_DissolveCutoff", 1);
        negativeCracks.material.SetFloat("_DissolveCutoff", 1);
    }

    void Update()
    {
        //Neutral background Transition
        if (_gameController.currentMeterValue > -_gameController.currentMeterSpreadValue &&
            _gameController.currentMeterValue < _gameController.currentMeterSpreadValue)
        {
            positiveSmiles.material.SetFloat("_DissolveCutoff", 1);
            negativeCracks.material.SetFloat("_DissolveCutoff", 1);
            if (neutralWallSprite.material.GetFloat("_DissolveCutoff") >= 1)
            {
                if (_isPositive)
                {
                    StartCoroutine(BackgroundTransitionAnimation(positiveWallSprite, neutralWallSprite));
                    StartCoroutine(BackgroundTransitionAnimation(positiveShelfSprite, neutralShelfSprite));
                    _isPositive = false;
                }

                if (_isNegative)
                {
                    StartCoroutine(BackgroundTransitionAnimation(negativeWallSprite, neutralWallSprite));
                    StartCoroutine(BackgroundTransitionAnimation(negativeShelfSprite, neutralShelfSprite));
                    _isNegative = false;
                }
            }
        }

        //Negative background transition
        if (_gameController.currentMeterValue <= -_gameController.currentMeterSpreadValue)
        {
            negativeCracks.material.SetFloat("_DissolveCutoff",
                Mathf.Clamp((_gameController.currentMeterSpreadValue * 1) / Mathf.Abs(_gameController.currentMeterValue), 0, 1));
            if (negativeWallSprite.material.GetFloat("_DissolveCutoff") >= 1)
            {
                StartCoroutine(BackgroundTransitionAnimation(neutralWallSprite, negativeWallSprite));
                StartCoroutine(BackgroundTransitionAnimation(neutralShelfSprite, negativeShelfSprite));
                _isNegative = true;
            }
        }


        //Positive background transition
        if (_gameController.currentMeterValue >= _gameController.currentMeterSpreadValue)
        {
            positiveSmiles.material.SetFloat("_DissolveCutoff",
                Mathf.Clamp((_gameController.currentMeterSpreadValue * 1) / _gameController.currentMeterValue, 0, 1));
            if (positiveWallSprite.material.GetFloat("_DissolveCutoff") >= 1)
            {
                StartCoroutine(BackgroundTransitionAnimation(neutralWallSprite, positiveWallSprite));
                StartCoroutine(BackgroundTransitionAnimation(neutralShelfSprite, positiveShelfSprite));
                _isPositive = true;
            }
        }
    }

    //Transition dissolve animation
    IEnumerator BackgroundTransitionAnimation(SpriteRenderer previousBackground, SpriteRenderer nextBackground)
    {
        int order = nextBackground.sortingOrder;
        nextBackground.sortingOrder = previousBackground.sortingOrder;
        previousBackground.sortingOrder = order;
        float cutOutStep = 0f;
        while (cutOutStep <= 1)
        {
            cutOutStep += transitionSpeed * Time.deltaTime;
            nextBackground.material.SetFloat("_DissolveCutoff", Mathf.Clamp(1 - cutOutStep, 0, 1));
            yield return null;
        }

        previousBackground.material.SetFloat("_DissolveCutoff", 1);
    }

    
    //Switch background Props between days/levels
    public void ChangeBackgroundProps()
    {
        int randomIndex = Random.Range(0, backgroundPropsPrefabsList.Count);
        if (_currentProp != null)
        {
            Destroy(_currentProp);
        }

        _currentProp = Instantiate(backgroundPropsPrefabsList[randomIndex]);
        _currentProp.transform.position = new Vector3(1.2f, -0.35f, -10);
        _currentProp.transform.parent = GameObject.Find("Level").transform;
    }
}
