using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Slider = UnityEngine.UI.Slider;

public class BackgroundTransition : MonoBehaviour
{
    private GameManager _gameManager;
    public Slider meterSlider;
    public float transitionSpeed;
    public SpriteRenderer neutralWallSprite;
    public SpriteRenderer positiveWallSprite;
    public SpriteRenderer negativeWallSprite;
    public SpriteRenderer neutralShelfSprite;
    public SpriteRenderer positiveShelfSprite;
    public SpriteRenderer negativeShelfSprite;
    public SpriteRenderer positiveSmiles;
    public SpriteRenderer negativeCracks;
    public List<GameObject> backgroundPropsList;
    public GameObject lastProp;
    private bool _isPositive;
    private bool _isNegative;
    void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();
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

    // Update is called once per frame
    void Update()
    {
        //Neutral background Transition
        if (meterSlider.value > -_gameManager.currentMeterSpreadValue &&
            meterSlider.value < _gameManager.currentMeterSpreadValue)
        {
            positiveSmiles.material.SetFloat("_DissolveCutoff", 1);
            negativeCracks.material.SetFloat("_DissolveCutoff", 1);
            //negativeDirtiness.material.SetFloat("_DissolveCutoff", 1 - Mathf.Clamp((_gameManager.currentMeterSpreadValue*1)/Mathf.Abs(meterSlider.value), 0 , 1 ));
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
        if (meterSlider.value <= -_gameManager.currentMeterSpreadValue)
        {
            negativeCracks.material.SetFloat("_DissolveCutoff", Mathf.Clamp((_gameManager.currentMeterSpreadValue*1)/Mathf.Abs(meterSlider.value), 0 , 1 ));
            if (negativeWallSprite.material.GetFloat("_DissolveCutoff") >= 1)
            {
                StartCoroutine(BackgroundTransitionAnimation(neutralWallSprite, negativeWallSprite));
                StartCoroutine(BackgroundTransitionAnimation(neutralShelfSprite, negativeShelfSprite));
                _isNegative = true;
            }
        }
        
        
        //Positive background transition
        if (meterSlider.value >= _gameManager.currentMeterSpreadValue)
        {
            positiveSmiles.material.SetFloat("_DissolveCutoff", Mathf.Clamp((_gameManager.currentMeterSpreadValue*1)/meterSlider.value, 0 , 1 ));
            if (positiveWallSprite.material.GetFloat("_DissolveCutoff") >= 1)
            {
                StartCoroutine(BackgroundTransitionAnimation(neutralWallSprite, positiveWallSprite));
                StartCoroutine(BackgroundTransitionAnimation(neutralShelfSprite, positiveShelfSprite));
                _isPositive = true;
            }
        }
    }

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

    public void ChangeBackgroundProps()
    {
        int randomIndex = Random.Range(0, backgroundPropsList.Count);
        if (lastProp != null)
        {
            Destroy(lastProp);
        }
        lastProp = Instantiate(backgroundPropsList[randomIndex]);
        lastProp.transform.parent = GameObject.Find("Level").transform;
    }
}
