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
    public List<ThoughtsAttributesScriptableObject> thoughtsAttributes;
    
    [Space(30, order = 3)]
    [Header("Thought current attributes", order = 4)]
    [Space(15, order = 5)]
    
    [Tooltip("Drop velocity in units/second")]
    public float Speed;

    [Tooltip("Drop velocity combo increment in units/second")]
    public float SpeedComboIncrement;

    [Tooltip("Horizontal minimum force value")]
    public float minHorizontalForceValue;
    [Tooltip("Horizontal maximum force value")]
    public float maxHorizontalForceValue;

    public float minHorizontalForceTriggerTimeInterval;
    public float maxHorizontalForceTriggerTimeInterval;
    
    [Tooltip("Current thought category")]
    public string category;
    
    [Tooltip("Current internal score value")]
    public float value;
    
    [Tooltip("Current color")]
    public Color color;
    
    [Tooltip("Current thought visual text")]
    public string thought;
    
    private ScoreEvent _score;
    private UnityEvent _miss;
    private GameManager _gameManager;
    private TextMeshPro _text;
    private Transform _currentTransform;
    private Vector3 _currentPosition;
    private Rigidbody2D _rb;
    private int _randomIndex;
    private float _timer;
    private float _timeRandomInterval;
    private int _colorPulseSpeed;
    
    void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _text = GetComponentInChildren<TextMeshPro>();
        _currentTransform = GetComponent<Transform>();
        _rb = GetComponent<Rigidbody2D>();

        //Adds an event listener that executes the score method on invoke of the ScoreEvent
        _score = new ScoreEvent();
        _miss = new UnityEvent();
        _score.AddListener(_gameManager.Score);
        _miss.AddListener(_gameManager.OnMissChangeDropSpeed);
        
        //......I think its obvious what it does :-)
        ResetBehaviour();
    }

    private void Update()
    {
        //Drop movement
        _currentPosition = _currentTransform.position;
        _currentPosition = new Vector3(_currentPosition.x, _currentPosition.y - ((Speed + SpeedComboIncrement)*Time.deltaTime), _currentPosition.z);
        _currentTransform.position = _currentPosition;
    }

    void FixedUpdate()
    {
        
        //Add horizontal forces on random time intervals
        _timer += Time.deltaTime;
        if (_timer >= _timeRandomInterval)
        {
            _rb.AddForce(Vector2.right*Random.Range(minHorizontalForceValue,maxHorizontalForceValue)*Random.Range(-1,2), ForceMode2D.Impulse);
            _timer = 0;
            _timeRandomInterval = Random.Range(minHorizontalForceTriggerTimeInterval, maxHorizontalForceTriggerTimeInterval);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        //Tests if it collides with the player and if its true invoke the ScoreEvent and deactivates this object
        if (other.CompareTag("Player"))
        {
            _score.Invoke(value);
            gameObject.SetActive(false);
        }

        //Tests if it collides with the kill/despawn trigger out of level bounds and if it's true deactivates this object
        if (other.CompareTag("DespawnTrigger"))
        {
            _miss.Invoke();
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
    public void ResetBehaviour()
    {
        _randomIndex = Random.Range(0, thoughtsAttributes.Count);
        int categoryIndex = _randomIndex;
        category = thoughtsAttributes[categoryIndex].category;
        value = thoughtsAttributes[categoryIndex].value;
        color = thoughtsAttributes[categoryIndex].color;
        _text.fontMaterial.SetColor(ShaderUtilities.ID_UnderlayColor, color);
        _randomIndex = Random.Range(0, thoughtsAttributes[categoryIndex].thoughts.Count);
        thought = thoughtsAttributes[categoryIndex].thoughts[_randomIndex];
        _text.text = thought;
        _timer = 0;
        _timeRandomInterval = Random.Range(minHorizontalForceTriggerTimeInterval, maxHorizontalForceTriggerTimeInterval);
        _rb.velocity = Vector2.zero;
        _rb.angularVelocity = 0;
    }
}


//Custom UnityEvent ScoreEvent that can pass the score value
public class ScoreEvent : UnityEvent<float>
{
    
}
