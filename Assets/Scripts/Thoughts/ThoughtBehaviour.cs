using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using RDG;
using UnityEngine.Rendering.Universal;
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
    public float horizontalForceComboIncrementValue;

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
    private GameManager _gameManager;
    private TextMeshPro _text;
    private Transform _currentTransform;
    private Vector3 _currentPosition;
    private Rigidbody2D _rb;
    private BoxCollider2D _collider;
    public int currentIndex = 0;
    private int _randomIndex;
    private float _horizontalForceTimer;
    private float _randomTimeInterval;

    private PulseAnimation _pulseAnimation;
    private FadeAnimation fadeAnimation;

    public virtual void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _text = GetComponentInChildren<TextMeshPro>();
        _currentTransform = GetComponent<Transform>();
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<BoxCollider2D>();
        _pulseAnimation = GetComponent<PulseAnimation>();
        fadeAnimation = GetComponent<FadeAnimation>();
        _catchEvent = new CatchEvent();
        _catchEvent.AddListener(_gameManager.OnCatchEvent);
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
            Vibration.Vibrate(250);
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
        category = thoughtsAttributesList[currentIndex].category;
        if (category == "Doubt")
        {
            StartCoroutine(Doubt());
            _pulseAnimation.animate = thoughtsAttributesList[currentIndex].animate;
            _pulseAnimation.colorPulseTime = thoughtsAttributesList[currentIndex].animationCycleTime;
            _horizontalForceTimer = 0;
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
            _horizontalForceTimer = 0;
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
        _horizontalForceTimer = 0;
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
        StartCoroutine(fadeAnimation.AnimateFade(textColor, outerColor));
    }

    //Add Horizontal force on a random time interval
    public IEnumerator HorizontalMovement()
    {
        while (hasHorizontalMovement)
        {
            float randomSign = Random.Range(0, 2) * 2 - 1;
            Vector2 randomForce = Vector2.right * (Random.Range(minHorizontalForceValue, maxHorizontalForceValue) * randomSign);
            _rb.AddForce(randomForce, ForceMode2D.Impulse);
            _horizontalForceTimer = 0;
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
        category = thoughtsAttributesList[currentIndex].category;
        int index = Random.Range(0, thoughtsAttributesList.Count);
        while (thoughtsAttributesList[index].category == "Doubt" || thoughtsAttributesList[index].category == "Surprise")
        {
            yield return null;
        }
        Debug.Log(thoughtsAttributesList[index].category + " " + index);
        _randomIndex = Random.Range(0, thoughtsAttributesList[index].thoughts.Count);
        thoughtString = thoughtsAttributesList[index].thoughts[_randomIndex];
        _text.text = thoughtString;
        scoreValue = thoughtsAttributesList[index].defaultValue;
        textColor = thoughtsAttributesList[currentIndex].textColor;
        outerColor = thoughtsAttributesList[currentIndex].outerColor;
        _text.fontMaterial.SetColor(ShaderUtilities.ID_FaceColor, textColor);
        _text.fontMaterial.SetColor(ShaderUtilities.ID_UnderlayColor, outerColor);
        _text.fontMaterial.SetColor("_DissolveEdgeColor", outerColor);
        _text.ForceMeshUpdate();
        _collider.enabled = true;
        _collider.offset = Vector2.zero;
        _collider.size = new Vector2(_text.GetRenderedValues(true).x, _text.GetRenderedValues(true).y);
        float fallTime = dropSpeed*((_currentPosition.y + Math.Abs(ScreenProperties.currentScreenCoords.yMin)) - (_currentPosition.y - (ScreenProperties.currentScreenCoords.yMax + Math.Abs(ScreenProperties.currentScreenCoords.yMin))));
        yield return new WaitForSeconds(1.5f);
        textColor = thoughtsAttributesList[index].textColor;
        outerColor = thoughtsAttributesList[index].outerColor;
        _text.fontMaterial.SetColor(ShaderUtilities.ID_FaceColor, textColor);
        _text.fontMaterial.SetColor(ShaderUtilities.ID_UnderlayColor, outerColor);
        _text.fontMaterial.SetColor("_DissolveEdgeColor", outerColor);
    }
}



//Custom UnityEvent ScoreEvent that can pass the score value
public class CatchEvent : UnityEvent<float>
{
    
}
