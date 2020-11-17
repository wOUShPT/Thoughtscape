using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class ThoughtBehaviour : MonoBehaviour
{
    [Space(30, order = 0)]
    [Header("Thoughts attributes data containers", order = 1)]
    [Space(15, order = 2)]
    
    [Tooltip("List of Thoughts attributes data containers (ScriptableObjects)")]
    public List<ThoughtsAttributesScriptableObject> thoughtsAttributesList;

    [Space(30, order = 3)]
    [Header("Thought current attributes", order = 4)]
    [Space(15, order = 5)]
    
    [Tooltip("Drop velocity in units/second")]
    public float dropSpeed;

    [Tooltip("Drop velocity combo increment in units/second")]
    public float dropSpeedComboIncrement;

    [Tooltip("Horizontal minimum force value")]
    public float minHorizontalForceValue;
    [Tooltip("Horizontal maximum force value")]
    public float maxHorizontalForceValue;
    public float horizontalForceIncrementValue;

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
    
    private ScoreEvent _scoreEvent;
    private UnityEvent _missEvent;
    private GameManager _gameManager;
    private TextMeshPro _text;
    private Transform _currentTransform;
    private Vector3 _currentPosition;
    private Rigidbody2D _rb;
    private BoxCollider2D _collider;
    private int _randomIndex;
    private float _timer;
    private float _randomTimeInterval;
    private bool _canAddHorizontalForce;

    void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _text = GetComponentInChildren<TextMeshPro>();
        _currentTransform = GetComponent<Transform>();
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<BoxCollider2D>();

        //Adds an event listener that executes the score method on invoke of the ScoreEvent
        _scoreEvent = new ScoreEvent();
        _missEvent = new UnityEvent();
        _scoreEvent.AddListener(_gameManager.OnScoreEvent);
        _missEvent.AddListener(_gameManager.OnMissEvent);
    }

    private void Update()
    {
        //Drop movement
        _currentPosition = _currentTransform.position;
        _currentPosition = new Vector3(_currentPosition.x, _currentPosition.y - ((dropSpeed + dropSpeedComboIncrement)*Time.deltaTime), _currentPosition.z);
        _currentTransform.position = _currentPosition;
        
        //Add horizontal force timer
        _timer += Time.deltaTime;
        if (_timer >= _randomTimeInterval)
        {
            _canAddHorizontalForce = true;
        }
    }

    void FixedUpdate()
    {
        
        //Add horizontal force
        if (_canAddHorizontalForce)
        {
            _rb.AddForce((Random.Range(0, 2) * 2 - 1) * Vector2.right * Random.Range(minHorizontalForceValue + horizontalForceIncrementValue ,maxHorizontalForceValue + horizontalForceIncrementValue), ForceMode2D.Impulse);
            _timer = 0;
            _randomTimeInterval = Random.Range(minHorizontalForceTriggerTimeInterval, maxHorizontalForceTriggerTimeInterval);
            _canAddHorizontalForce = false;
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        //Tests if it collides with the player and if its true invoke the ScoreEvent and deactivates this object
        if (other.CompareTag("Player"))
        {
            _scoreEvent.Invoke(scoreValue);
            gameObject.SetActive(false);
        }

        //Tests if it collides with the kill/despawn trigger out of level bounds and if it's true deactivates this object
        if (other.CompareTag("Despawn Trigger"))
        {
            _missEvent.Invoke();
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
    public void ResetBehaviour(int index)
    {
        category = thoughtsAttributesList[index].category;
        scoreValue = thoughtsAttributesList[index].value;
        textColor = thoughtsAttributesList[index].textColor;
        outerColor = thoughtsAttributesList[index].outerColor;
        _text.fontMaterial.SetColor(ShaderUtilities.ID_FaceColor, textColor);
        _text.fontMaterial.SetColor(ShaderUtilities.ID_UnderlayColor, outerColor);
        _randomIndex = Random.Range(0, thoughtsAttributesList[index].thoughts.Count);
        thoughtString = thoughtsAttributesList[index].thoughts[_randomIndex];
        _text.text = thoughtString;
        _collider.offset = Vector2.zero;
        _collider.size = new Vector2(_text.GetPreferredValues().x, _text.GetPreferredValues().y);
        _timer = 0;
        _randomTimeInterval = Random.Range(minHorizontalForceTriggerTimeInterval, maxHorizontalForceTriggerTimeInterval);
        _currentTransform.rotation = Quaternion.Euler(0,0,0);
        _rb.velocity = Vector2.zero;
        _rb.angularVelocity = 0;
        _canAddHorizontalForce = false;
    }
}


//Custom UnityEvent ScoreEvent that can pass the score value
public class ScoreEvent : UnityEvent<float>
{
    
}
