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
    
    [Tooltip("Current thought category")]
    public string category;
    
    [Tooltip("Current internal score value")]
    public float value;
    
    [Tooltip("Current color")]
    public Color color;
    
    [Tooltip("Current thought visual text")]
    public string thought;
    
    private ScoreEvent _score;
    private ScoreManager _scoreManager;
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
        _scoreManager = FindObjectOfType<ScoreManager>();
        _text = GetComponentInChildren<TextMeshPro>();
        _currentTransform = GetComponent<Transform>();
        _rb = GetComponent<Rigidbody2D>();

        //Adds an event listener that executes the score method on invoke of the ScoreEvent
        _score = new ScoreEvent();
        _score.AddListener(_scoreManager.Score);
        
        //......I think its obvious what it does :-)
        ResetBehaviour();
    }

    private void Update()
    {
        //Drop movement
        _currentPosition = _currentTransform.position;
        _currentPosition = new Vector3(_currentPosition.x, _currentPosition.y - (Speed*Time.deltaTime), _currentPosition.z);
        _currentTransform.position = _currentPosition;
    }

    void FixedUpdate()
    {
        
        //Add horizontal forces on random time intervals
        _timer += Time.deltaTime;
        if (_timer >= _timeRandomInterval)
        {
            _rb.AddForce(Vector2.right*Random.Range(-0.3f,0.3f), ForceMode2D.Impulse);
            _timer = 0;
            _timeRandomInterval = Random.Range(0.8f, 1.2f);
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
            gameObject.SetActive(false);
        }
    }

    //Reset the Thought behaviour to the default values and set its attributes
    public void ResetBehaviour()
    {
        SetAttributes();
        SetPosition();
    }
    
    //Set the Thoughts attributes randomly, based on a collection of Thoughts attributes data containers (ScriptableObjects)
    private void SetAttributes()
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
    }

    //set default movement and position related values
    private void SetPosition()
    {
        _timer = 0;
        _timeRandomInterval = Speed/2;
        _currentPosition = new Vector3(Random.Range(-2f, 2f), 5.3f, 0);
        _currentTransform.position = _currentPosition;
        _rb.velocity = Vector2.zero;
        _rb.angularVelocity = 0;
    }

}


//Custom UnityEvent ScoreEvent that can pass the score value
public class ScoreEvent : UnityEvent<float>
{
    
}
