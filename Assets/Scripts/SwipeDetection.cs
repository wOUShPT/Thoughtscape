using System;
using UnityEngine;

public class SwipeDetection : MonoBehaviour
{
    [SerializeField]
    private float _minDistance = 0.2f;
    [SerializeField]
    private float _maxtime = 1f;
    [SerializeField, Range(0, 1)]
    private float _directionThreshold = 0.9f;
    private InputManager _inputManager;
    private Vector2 _startPosition;
    private float _startTime;
    private Vector2 _endPosition;
    private float _endTime;
    void Awake()
    {
        _inputManager = FindObjectOfType<InputManager>();
    }

    private void OnEnable()
    {
        _inputManager.onStartTouch.AddListener(SwipeStart);
        _inputManager.onEndTouch.AddListener(SwipeEnd);
    }

    private void OnDisable()
    {
        _inputManager.onStartTouch.RemoveListener(SwipeStart);
        _inputManager.onEndTouch.RemoveListener(SwipeEnd);
    }

    private void SwipeStart(Vector2 position, float time)
    {
        _startPosition = position;
        _startTime = time;
    }
    
    private void SwipeEnd(Vector2 position, float time)
    {
        _endPosition = position;
        _endTime = time;
        DetectSwipe();
    }

    private void DetectSwipe()
    {
        if (Vector3.Distance(_startPosition, _endPosition) >= _minDistance && (_endTime - _startTime) <= _maxtime)
        {
            Debug.DrawLine(_startPosition,_endPosition, Color.red, 5f);
            Vector3 direction = _endPosition - _startPosition;
            Vector2 direction2D = new Vector2(direction.x, direction.y). normalized;
            SwipeDirection(direction2D);
        }
    }

    private void SwipeDirection(Vector2 direction)
    {
        if (Vector2.Dot(Vector2.up, direction) > _directionThreshold)
        {
            Debug.Log("Swipe Up");
        }
        
        if (Vector2.Dot(Vector2.left, direction) > _directionThreshold)
        {
            Debug.Log("Swipe left");
        }
        
        if (Vector2.Dot(Vector2.right, direction) > _directionThreshold)
        {
            Debug.Log("Swipe right");
        }
        
        if (Vector2.Dot(Vector2.down, direction) > _directionThreshold)
        {
            Debug.Log("Swipe down");
        }
    }
}
