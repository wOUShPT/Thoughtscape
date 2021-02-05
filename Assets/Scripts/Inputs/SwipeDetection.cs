using UnityEngine;
using UnityEngine.Events;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class SwipeDetection : MonoBehaviour
{
    [Tooltip("Minimum registered swipe distance")]
    public float minDistance = 0.2f;
    [Tooltip("Maximum registered swipe time")]
    public float maxTime = 1f;
    [Range(0, 1), Tooltip("swipeEvent direction threshold")]
    public float directionThreshold = 0.9f;
    private InputManager _inputManager;
    private Vector2 _startPosition;
    private float _startTime;
    private Vector2 _endPosition;
    private float _endTime;
    public SwipeEvent swipeEvent;
    void Awake()
    {
        _inputManager = FindObjectOfType<InputManager>();
    }

    private void OnEnable()
    {
        _inputManager.onStartTouchEvent.AddListener(SwipeStart);
        _inputManager.onStartTouchEvent.AddListener(SwipeEnd);
        swipeEvent = new SwipeEvent();
    }

    private void OnDisable()
    {
        _inputManager.onStartTouchEvent.RemoveListener(SwipeStart);
        _inputManager.onStartTouchEvent.RemoveListener(SwipeEnd);
    }

    private void SwipeStart(TouchPhase phase, Vector2 position, float time)
    {
        if (phase == TouchPhase.Began)
        {
            _startPosition = position;
            _startTime = time;
        }
    }
    
    private void SwipeEnd(TouchPhase phase, Vector2 position, float time)
    {
        if (phase == TouchPhase.Ended)
        {
            _endPosition = position;
            _endTime = time;
            DetectSwipe();
        }
    }

    private void DetectSwipe()
    {
        if (Vector3.Distance(_startPosition, _endPosition) >= minDistance && (_endTime - _startTime) <= maxTime)
        {
            Debug.DrawLine(_startPosition,_endPosition, Color.red, 5f);
            Vector3 direction = _endPosition - _startPosition;
            Vector2 direction2D = new Vector2(direction.x, direction.y).normalized;
            SwipeDirection(direction2D);
        }
    }

    private void SwipeDirection(Vector2 direction)
    {
        if (Vector2.Dot(Vector2.up, direction) > directionThreshold)
        {
           swipeEvent.Invoke(Vector2.up);
        }
        
        if (Vector2.Dot(Vector2.left, direction) > directionThreshold)
        {
           swipeEvent.Invoke(Vector2.left);
        }
        
        if (Vector2.Dot(Vector2.right, direction) > directionThreshold)
        {
            swipeEvent.Invoke(Vector2.right);
        }
        
        if (Vector2.Dot(Vector2.down, direction) > directionThreshold)
        {
            swipeEvent.Invoke(Vector2.down);
        }
    }

    public class SwipeEvent : UnityEvent<Vector2>
    {
        
    }
}
