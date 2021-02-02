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

    public BoxCollider2D textCollider;
    private Transform _currentTransform;
    private Vector3 _currentPosition;
    public Rigidbody2D rb;
    
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
    private InputManager _inputManager;
    private FadeAnimation _fadeAnimation;
    private PulseAnimation _pulseAnimation;
    public ThoughtsAttributesScriptableObject currentThoughtAttributes;
    private RaycastHit2D _hit;

    private ThoughtDefaultBehaviour _thoughtDefaultBehaviour;
    private ThoughtInsecurityBehaviour _thoughtInsecurityBehaviour;
    void Awake()
    {
        _currentTransform = GetComponent<Transform>();
        _catchEvent = new CatchEvent();
        _inputManager = FindObjectOfType<InputManager>();
        _gameController = FindObjectOfType<GameController>();
        _catchEvent.AddListener(_gameController.OnCatchEvent);
        _fadeAnimation = GetComponent<FadeAnimation>();
        _thoughtDefaultBehaviour = GetComponent<ThoughtDefaultBehaviour>();
        _thoughtInsecurityBehaviour = GetComponent<ThoughtInsecurityBehaviour>();
    }
    
    private void Update()
    {
        //Drop movement
        _currentPosition = _currentTransform.position;
        _currentPosition = new Vector3(_currentPosition.x, _currentPosition.y - ((dropSpeed*dropSpeedMultiplier)*Time.deltaTime), _currentPosition.z);
        _currentTransform.position = _currentPosition;

        /*_hit = Physics2D.Raycast(transform.position, Vector2.right, 10);
        if (_hit.transform.CompareTag("LevelLimits") &&
            Vector2.Distance(_hit.transform.position, transform.position) <= 0.05)
        {
            rb.AddForce(Random.Range(minHorizontalForceValue,maxHorizontalForceValue)*Vector2.left, ForceMode2D.Impulse);
        }*/
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
        if (currentThoughtAttributes.category == "Doubt")
        {
            _thoughtInsecurityBehaviour.ResetBehaviour();
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
            rb.AddForce(randomForce, ForceMode2D.Impulse);
            horizontalForceTriggerRandomTimeInterval = Random.Range(minHorizontalForceTriggerTimeInterval, maxHorizontalForceTriggerTimeInterval);
            yield return new WaitForSeconds(horizontalForceTriggerRandomTimeInterval);
            yield return null;
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        DeSpawn(other);
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("LevelLimits"))
        {
            
        }
    }

    public virtual void DeSpawn(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            StopCoroutine(HorizontalMovement());
            //StopCoroutine(Surprise());
            _catchEvent.Invoke(scoreValue);
            Fade();
        }

        //Tests if it collides with the kill/despawn trigger out of level bounds and if it's true deactivates this object
        if (col.CompareTag("Despawn Trigger"))
        {
            StopCoroutine(HorizontalMovement());
            //StopCoroutine(Surprise());
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
