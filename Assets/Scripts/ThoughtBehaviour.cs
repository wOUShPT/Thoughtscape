using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class ThoughtBehaviour : MonoBehaviour
{
    public List<ThoughtsAttributesScriptableObject> thoughtsAttributes;
    public float Speed;
    public float colorDilatationSpeed;
    public string category;
    public Color color;
    public string thought;
    public ScoreEvent score;
    public LayerMask thoughtsLayer;
    private ScoreManager _scoreManager;
    private TextMeshPro _text;
    private Transform _currentTransform;
    private Vector3 _currentPosition;
    private Rigidbody2D _rb;
    private Collider2D _collider2D;
    private int _randomIndex;
    private float _timer;
    private float _timeRandomInterval;
    private float _dilatationValue;
    private int _dilatationSpeedSign;

    void Awake()
    {
        Physics2D.IgnoreLayerCollision(9, 8, true);
        _dilatationValue = -1;
        _scoreManager = FindObjectOfType<ScoreManager>();
        _text = GetComponentInChildren<TextMeshPro>();
        _currentTransform = GetComponent<Transform>();
        _rb = GetComponent<Rigidbody2D>();
        _collider2D = GetComponent<Collider2D>();
        score = new ScoreEvent();
        score.AddListener(_scoreManager.Score);
        ResetBehaviour();
    }

    private void Update()
    {
        _currentPosition = _currentTransform.position;
        _currentPosition = new Vector3(_currentPosition.x, _currentPosition.y - (Speed*Time.deltaTime), _currentPosition.z);
        _currentTransform.position = _currentPosition;
    }

    void FixedUpdate()
    {
        PulseAnimation();
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
        if (other.CompareTag("Player"))
        {
            score.Invoke(_text.color);
            gameObject.SetActive(false);
        }

        if (other.CompareTag("DespawnTrigger"))
        {
            gameObject.SetActive(false);
        }
    }
    
    private void SetAttributes()
    {
        _randomIndex = Random.Range(0, thoughtsAttributes.Count);
        int categoryIndex = _randomIndex;
        category = thoughtsAttributes[categoryIndex].category;
        color = thoughtsAttributes[categoryIndex].color;
        _text.fontMaterial.SetColor(ShaderUtilities.ID_UnderlayColor, color);
        _randomIndex = Random.Range(0, thoughtsAttributes[categoryIndex].thoughts.Count);
        thought = thoughtsAttributes[categoryIndex].thoughts[_randomIndex];
        _text.text = thought;
    }

    public void ResetBehaviour()
    {
        SetAttributes();
        _timer = 0;
        _dilatationValue = -1;
        _timeRandomInterval = Speed/2;
        _currentPosition = new Vector3(Random.Range(-2f, 2f), 5.3f, 0);
        _currentTransform.position = _currentPosition;
        _rb.velocity = Vector2.zero;
        _rb.angularVelocity = 0;
        //_rb.velocity = Vector2.down*Speed; 
    }

    void PulseAnimation()
    {
        _dilatationValue = _dilatationValue + (colorDilatationSpeed * _dilatationSpeedSign * Time.deltaTime);
        _dilatationValue = Mathf.Clamp(_dilatationValue, -1, 1);
        _text.fontMaterial.SetFloat(ShaderUtilities.ID_UnderlayDilate, _dilatationValue);
        if (_dilatationValue == -1)
        {
            _dilatationSpeedSign = 1;
        }

        if (_dilatationValue == 1)
        {
            _dilatationSpeedSign = -1;
        }
    }
}

public class ScoreEvent : UnityEvent<Color>
{
    
}
