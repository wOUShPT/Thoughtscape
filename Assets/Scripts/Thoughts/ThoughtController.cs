using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

public class ThoughtController : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Current thought category")]
    public string category;
    
    [SerializeField]
    [Tooltip("Current internal score value")]
    public float scoreValue;

    [Tooltip("Current text color")]
    public Color textColor;
    
    [Tooltip("Current glow color")]
    public Color outerColor;

    [Tooltip("Current thought visual text")]
    public string thoughtString;

    [Tooltip("Current thought index")]
    public int currentIndex;
    
    [Tooltip("TextMeshPro Component")]
    public TextMeshPro textMeshPro;

    [Tooltip("Text BoxCollider2D Component")]
    public BoxCollider2D textCollider;
    private Transform _currentTransform;
    private Vector3 _currentPosition;
    
    [Tooltip("Text RigidBody2D Component")]
    public Rigidbody2D rigidBody2D;
    
    public float dropSpeed;
    public float dropSpeedMultiplier;
    public bool hasHorizontalMovement;
    public float minHorizontalForceValue;
    public float maxHorizontalForceValue;
    public float horizontalForceTriggerRandomTimeInterval;
    public float minHorizontalForceTriggerTimeInterval;
    public float maxHorizontalForceTriggerTimeInterval;
    private CatchEvent _catchEvent;
    private GameController _gameController;
    private FadeAnimation _fadeAnimation;
    private PulseAnimation _pulseAnimation;
    public ThoughtsAttributesScriptableObject currentThoughtAttributes;
    private RaycastHit2D _hit;

    private ThoughtDefaultBehaviour _thoughtDefaultBehaviour;
    private ThoughtInsecurityBehaviour _thoughtInsecurityBehaviour;
    private ThoughtConfusionBehaviour _thoughtConfusionBehaviour;
    void Awake()
    {
        _currentTransform = GetComponent<Transform>();
        textCollider = GetComponent<BoxCollider2D>();
        rigidBody2D = GetComponent<Rigidbody2D>();
        _catchEvent = new CatchEvent();
        _gameController = FindObjectOfType<GameController>();
        _catchEvent.AddListener(_gameController.OnCatchEvent);
        _fadeAnimation = GetComponent<FadeAnimation>();
        _thoughtDefaultBehaviour = GetComponent<ThoughtDefaultBehaviour>();
        _thoughtInsecurityBehaviour = GetComponent<ThoughtInsecurityBehaviour>();
        _thoughtConfusionBehaviour = GetComponent<ThoughtConfusionBehaviour>();
    }
    
    private void Update()
    {
        //Drop movement
        _currentPosition = _currentTransform.position;
        _currentPosition = new Vector3(_currentPosition.x, _currentPosition.y - ((dropSpeed*dropSpeedMultiplier)*Time.deltaTime), _currentPosition.z);
        _currentTransform.position = _currentPosition;

        if (!hasHorizontalMovement)
        {
            return;
        }
        
        _hit = Physics2D.Raycast(new Vector3(transform.position.x + textCollider.size.x/2 + 0.005f, transform.position.y, transform.position.z), Vector2.right, 20);
        if (_hit.collider.CompareTag("LevelLimits") && Vector2.Distance(_hit.transform.position, transform.position) <= 0.25f)
        {
            rigidBody2D.AddForce(Vector2.left*0.1f, ForceMode2D.Impulse);
            return;
        }

        _hit = Physics2D.Raycast(new Vector3(transform.position.x - textCollider.size.x/2 - 0.005f, transform.position.y, transform.position.z), Vector2.left, 20);
        if (_hit.collider.CompareTag("LevelLimits") && Vector2.Distance(_hit.transform.position, transform.position) <= 0.25f)
        {
            rigidBody2D.AddForce(Vector2.right*0.1f, ForceMode2D.Impulse);
        }
    }

    public void ResetPosition()
    {
        _currentTransform.position = new Vector3(Random.Range(ScreenProperties.currentScreenCoords.xMin+0.5f, ScreenProperties.currentScreenCoords.xMax-0.5f),ScreenProperties.currentScreenCoords.yMax+1,0);
        _currentPosition = _currentTransform.position;
        _currentTransform.localRotation = Quaternion.identity;
    }

    public void SetThought(ThoughtsAttributesScriptableObject thoughtAttributes)
    {
        currentThoughtAttributes = thoughtAttributes;
        if (currentThoughtAttributes.category == "Insecurity")
        {
            _thoughtInsecurityBehaviour.ResetBehaviour();
            return;
        }

        if (currentThoughtAttributes.category == "Confusion")
        {
            _thoughtConfusionBehaviour.ResetBehaviour();
            return;
        }
        _thoughtDefaultBehaviour.ResetBehaviour();
    }
    
    //Add Horizontal force on a random time interval
    public IEnumerator HorizontalMovement()
    {
        while (hasHorizontalMovement)
        {
            float randomSign = Random.Range(0, 2) * 2 - 1;
            Vector2 randomForce = Vector2.right * (Random.Range(minHorizontalForceValue, maxHorizontalForceValue) * randomSign);
            rigidBody2D.AddForce(randomForce, ForceMode2D.Impulse);
            horizontalForceTriggerRandomTimeInterval = Random.Range(minHorizontalForceTriggerTimeInterval, maxHorizontalForceTriggerTimeInterval);
            yield return new WaitForSeconds(horizontalForceTriggerRandomTimeInterval);
            yield return null;
        }

        yield return null;
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        DeSpawn(other);
    }
    

    public virtual void DeSpawn(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            _catchEvent.Invoke(scoreValue);
            Fade();
        }

        //Tests if it collides with the kill/despawn trigger out of level bounds and if it's true deactivates this object
        if (col.CompareTag("Despawn Trigger"))
        {
            gameObject.SetActive(false);
        }
    }

    //Start Fade animation
    public void Fade()
    {
        textCollider.enabled = false;
        StartCoroutine(_fadeAnimation.AnimateFade(textColor, outerColor));
    }
}


//Custom UnityEvent ScoreEvent that can pass the score value
public class CatchEvent : UnityEvent<float>
{
    
}
