﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using RDG;
using Random = UnityEngine.Random;

public class ThoughtBehaviour : MonoBehaviour
{
    [Space(30, order = 0)]
    [Header("Thoughts attributes data containers", order = 1)]
    [Space(15, order = 2)]
    
    [Tooltip("List of Thoughts attributes data containers (Scriptable objects)")]
    public List<ThoughtsAttributesScriptableObject> thoughtsAttributesList;
    [Space(15)]
    public ThoughtsAttributesScriptableObject _positiveDoubtThoughtAttributes;
    public ThoughtsAttributesScriptableObject _negativeDoubtThoughtAttributes;
    [Space(15)]
    public List<ThoughtsAttributesScriptableObject> thoughtsAttributesSurpriseList;

    [Space(30, order = 3)]
    [Header("Thought current attributes", order = 4)]
    [Space(15, order = 5)]
    
    [Tooltip("Drop velocity in units/second")]
    public float dropSpeed;
    [Tooltip("Drop velocity multiplier")]
    public float dropSpeedMultiplier;

    [Tooltip("Has horizontal movement?")]
    public bool hasHorizontalMovement;
    [Tooltip("Horizontal minimum force value")]
    public float minHorizontalForceValue;
    [Tooltip("Horizontal maximum force value")]
    public float maxHorizontalForceValue;

    public float minHorizontalForceTriggerTimeInterval;
    public float maxHorizontalForceTriggerTimeInterval;

    [Tooltip("Current thought category")]
    public string category;
    
    [Tooltip("Current internal score value")]
    public float scoreValue;

    [Tooltip("Current text color")]
    public Color textColor;
    
    [Tooltip("Current glow color")]
    public Color outerColor;

    [Tooltip("Current thought visual text")]
    public string thoughtString;
    
    private CatchEvent _catchEvent;
    private bool _canVibrate;
    private GameController _gameController;
    private InputManager _inputManager;
    private TextMeshPro _text;
    private Transform _currentTransform;
    private Vector3 _currentPosition;
    private Rigidbody2D _rb;
    private BoxCollider2D _collider;
    public int currentIndex = 0;
    private int _randomIndex;
    private float _randomTimeInterval;

    private PulseAnimation _pulseAnimation;
    private FadeAnimation _fadeAnimation;

    public virtual void Awake()
    {
        _gameController = FindObjectOfType<GameController>();
        _inputManager = FindObjectOfType<InputManager>();
        _text = GetComponentInChildren<TextMeshPro>();
        _currentTransform = GetComponent<Transform>();
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<BoxCollider2D>();
        _pulseAnimation = GetComponent<PulseAnimation>();
        _fadeAnimation = GetComponent<FadeAnimation>();
        _catchEvent = new CatchEvent();
        _catchEvent.AddListener(_gameController.OnCatchEvent);
    }

    private void Update()
    {
        //Drop movement
        _currentPosition = _currentTransform.position;
        _currentPosition = new Vector3(_currentPosition.x, _currentPosition.y - ((dropSpeed*dropSpeedMultiplier)*Time.deltaTime), _currentPosition.z);
        _currentTransform.position = _currentPosition;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //Tests if it collides with the player and if its true invoke the ScoreEvent and deactivates this object
        if (other.CompareTag("Player"))
        {
            StopCoroutine(HorizontalMovement());
            StopCoroutine(Doubt());
            StopCoroutine(Surprise());
            _catchEvent.Invoke(scoreValue);
            _inputManager.Vibrate();
            Fade();
        }

        //Tests if it collides with the kill/despawn trigger out of level bounds and if it's true deactivates this object
        if (other.CompareTag("Despawn Trigger"))
        {
            StopCoroutine(HorizontalMovement());
            StopCoroutine(Doubt());
            StopCoroutine(Surprise());
            gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("LevelLimits"))
        {
            _rb.AddForce(other.GetContact(0).normal*Random.Range(minHorizontalForceValue,maxHorizontalForceValue), ForceMode2D.Impulse);
        }
    }

    //Reset the Thoughts attributes randomly, based on a collection of Thoughts attributes data containers (ScriptableObjects) / reset other behaviour default values
    public virtual void ResetBehaviour()
    {
        _currentTransform.position = new Vector3(Random.Range(ScreenProperties.currentScreenCoords.xMin+0.5f, ScreenProperties.currentScreenCoords.xMax-0.5f),ScreenProperties.currentScreenCoords.yMax+1,0);
        _currentPosition = _currentTransform.position;
        category = thoughtsAttributesList[currentIndex].category;
        if (category == "Doubt")
        {
            StartCoroutine(Doubt());
            _pulseAnimation.animate = thoughtsAttributesList[currentIndex].animate;
            _pulseAnimation.colorPulseTime = thoughtsAttributesList[currentIndex].animationCycleTime;
            _randomTimeInterval = Random.Range(minHorizontalForceTriggerTimeInterval, maxHorizontalForceTriggerTimeInterval);
            _currentTransform.localRotation = Quaternion.identity;
            _rb.velocity = Vector2.zero;
            _rb.angularVelocity = 0;
            StartCoroutine(HorizontalMovement());
            return;
        }

        if (category == "Surprise")
        {
            StartCoroutine(Surprise());
            _pulseAnimation.animate = thoughtsAttributesList[currentIndex].animate;
            _pulseAnimation.colorPulseTime = thoughtsAttributesList[currentIndex].animationCycleTime;
            _randomTimeInterval = Random.Range(minHorizontalForceTriggerTimeInterval, maxHorizontalForceTriggerTimeInterval);
            _currentTransform.localRotation = Quaternion.identity;
            _rb.velocity = Vector2.zero;
            _rb.angularVelocity = 0;
            StartCoroutine(HorizontalMovement());
            return;
        }
        scoreValue = thoughtsAttributesList[currentIndex].defaultValue;
        textColor = thoughtsAttributesList[currentIndex].textColor;
        outerColor = thoughtsAttributesList[currentIndex].outerColor;
        _text.fontMaterial.SetColor(ShaderUtilities.ID_FaceColor, textColor);
        _text.fontMaterial.SetColor(ShaderUtilities.ID_UnderlayColor, outerColor);
        _text.fontMaterial.SetColor("_DissolveEdgeColor", outerColor);
        _randomIndex = Random.Range(0, thoughtsAttributesList[currentIndex].thoughts.Count);
        thoughtString = thoughtsAttributesList[currentIndex].thoughts[_randomIndex];
        _text.text = thoughtString;
        _pulseAnimation.animate = thoughtsAttributesList[currentIndex].animate;
        _pulseAnimation.colorPulseTime = thoughtsAttributesList[currentIndex].animationCycleTime;
        _text.ForceMeshUpdate();
        _collider.enabled = true;
        _collider.offset = Vector2.zero;
        _collider.size = new Vector2(_text.GetRenderedValues(true).x, _text.GetRenderedValues(true).y);
        _randomTimeInterval = Random.Range(minHorizontalForceTriggerTimeInterval, maxHorizontalForceTriggerTimeInterval);
        _currentTransform.localRotation = Quaternion.identity;
        _rb.velocity = Vector2.zero;
        _rb.angularVelocity = 0;
        StartCoroutine(HorizontalMovement());
    }
    
    //Initilaize Fade animation
    public void Fade()
    {
        _collider.enabled = false;
        StartCoroutine(_fadeAnimation.AnimateFade(textColor, outerColor));
    }

    //Add Horizontal force on a random time interval
    public IEnumerator HorizontalMovement()
    {
        while (hasHorizontalMovement)
        {
            float randomSign = Random.Range(0, 2) * 2 - 1;
            Vector2 randomForce = Vector2.right * (Random.Range(minHorizontalForceValue, maxHorizontalForceValue) * randomSign);
            _rb.AddForce(randomForce, ForceMode2D.Impulse);
            _randomTimeInterval = Random.Range(minHorizontalForceTriggerTimeInterval, maxHorizontalForceTriggerTimeInterval);
            yield return new WaitForSeconds(_randomTimeInterval);
            yield return null;
        }
    }
    
    
    IEnumerator Doubt()
    {
        _randomIndex = Random.Range(0, _positiveDoubtThoughtAttributes.thoughts.Count);
        yield return new WaitForSeconds(Random.Range(0,0.2f));
        while (true)
        {
            scoreValue = _positiveDoubtThoughtAttributes.defaultValue;
            textColor = _positiveDoubtThoughtAttributes.textColor;
            outerColor = _positiveDoubtThoughtAttributes.outerColor;
            _text.fontMaterial.SetColor(ShaderUtilities.ID_FaceColor, textColor);
            _text.fontMaterial.SetColor(ShaderUtilities.ID_UnderlayColor, outerColor);
            _text.fontMaterial.SetColor("_DissolveEdgeColor", outerColor);
            thoughtString = _positiveDoubtThoughtAttributes.thoughts[_randomIndex];
            _text.text = thoughtString;
            _text.ForceMeshUpdate();
            _collider.enabled = true;
            _collider.offset = Vector2.zero;
            _collider.size = new Vector2(_text.GetRenderedValues(true).x, _text.GetRenderedValues(true).y);
            yield return new WaitForSeconds(thoughtsAttributesList[currentIndex].animationCycleTime);
            scoreValue = _negativeDoubtThoughtAttributes.defaultValue;
            textColor = _negativeDoubtThoughtAttributes.textColor;
            outerColor = _negativeDoubtThoughtAttributes.outerColor;
            _text.fontMaterial.SetColor(ShaderUtilities.ID_FaceColor, textColor);
            _text.fontMaterial.SetColor(ShaderUtilities.ID_UnderlayColor, outerColor);
            _text.fontMaterial.SetColor("_DissolveEdgeColor", outerColor);
            thoughtString = _negativeDoubtThoughtAttributes.thoughts[_randomIndex];
            _text.text = thoughtString;
            _text.ForceMeshUpdate();
            _collider.enabled = true;
            _collider.offset = Vector2.zero;
            _collider.size = new Vector2(_text.GetRenderedValues(true).x, _text.GetRenderedValues(true).y);
            yield return new WaitForSeconds(thoughtsAttributesList[currentIndex].animationCycleTime);
            yield return null;
        }
    }

    IEnumerator Surprise()
    {
        Debug.Log(_currentPosition.y.ToString());
        int index = Random.Range(0, thoughtsAttributesSurpriseList.Count);
        scoreValue = thoughtsAttributesSurpriseList[index].defaultValue;
        textColor = thoughtsAttributesList[currentIndex].textColor;
        outerColor = thoughtsAttributesList[currentIndex].outerColor;
        _text.fontMaterial.SetColor(ShaderUtilities.ID_FaceColor, textColor);
        _text.fontMaterial.SetColor(ShaderUtilities.ID_UnderlayColor, outerColor);
        _text.fontMaterial.SetColor("_DissolveEdgeColor", outerColor);
        Debug.Log("waiting");
        _randomIndex = Random.Range(0, thoughtsAttributesList[currentIndex].thoughts.Count);
        thoughtString = thoughtsAttributesList[currentIndex].thoughts[_randomIndex];
        _text.text = thoughtString;
        _text.ForceMeshUpdate();
        _collider.enabled = false;
        _collider.offset = Vector2.zero;
        _collider.size = new Vector2(_text.GetRenderedValues(true).x, _text.GetRenderedValues(true).y);
        float timer = 0;
        while (_currentPosition.y > 0)
        {
            timer += Time.deltaTime;
            if (timer > 0.1f)
            {
                timer = 0;
                _randomIndex = Random.Range(0, thoughtsAttributesList[currentIndex].thoughts.Count);
                thoughtString = thoughtsAttributesList[currentIndex].thoughts[_randomIndex];
                _text.text = thoughtString;
            }
            yield return null;
        }
        yield return new WaitUntil(() => _currentPosition.y < 0);
        Debug.Log("Waited");
        _randomIndex = Random.Range(0, thoughtsAttributesSurpriseList[index].thoughts.Count);
        thoughtString = thoughtsAttributesSurpriseList[index].thoughts[_randomIndex];
        _text.text = thoughtString;
        textColor = thoughtsAttributesSurpriseList[index].textColor;
        outerColor = thoughtsAttributesSurpriseList[index].outerColor;
        _text.fontMaterial.SetColor(ShaderUtilities.ID_FaceColor, textColor);
        _text.fontMaterial.SetColor(ShaderUtilities.ID_UnderlayColor, outerColor);
        _text.fontMaterial.SetColor("_DissolveEdgeColor", outerColor);
        _text.ForceMeshUpdate();
        _collider.enabled = true;
        _collider.offset = Vector2.zero;
        _collider.size = new Vector2(_text.GetRenderedValues(true).x, _text.GetRenderedValues(true).y);
    }
}






//Custom UnityEvent ScoreEvent that can pass the score value
public class CatchEvent : UnityEvent<float>
{
    
}
