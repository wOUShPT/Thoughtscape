using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    public float movementSpeed;
    private Rigidbody2D _rigidBody2D;
    private Transform _playerTransform;
    private Vector3 _touchPosition;
    private Vector3 _movementDirection;
    private Vector3 _currentPosition;
    private Camera _mainCamera;
    private Touch _touchInput;

    void Start()
    {
        _rigidBody2D = GetComponent<Rigidbody2D>();
        _playerTransform = GetComponent<Transform>();
        _mainCamera = FindObjectOfType<Camera>();
    }

    /*void Update()
    {
        float keyboardInputDirection = Input.GetAxis("Horizontal");
        _rigidBody2D.velocity = new Vector2(keyboardInputDirection * (movementSpeed / 5), 0);
        _currentPosition = _playerTransform.position;
        if (Input.touchCount > 0 && _mainCamera.ScreenToWorldPoint(Input.touches[0].position).y < -3)
        {
            _touchInput = Input.GetTouch(0);
            _touchPosition = new Vector3(_mainCamera.ScreenToWorldPoint(_touchInput.position).x, _currentPosition.y, 0);
            _movementDirection = _touchPosition - _currentPosition;
            _rigidBody2D.velocity = new Vector2(_movementDirection.x, 0) * movementSpeed;
            _playerTransform.position = new Vector3(_playerTransform.position.x, _currentPosition.y, _currentPosition.z);
            if (_touchInput.phase == TouchPhase.Ended)
            {
                _rigidBody2D.velocity = Vector2.zero;
            }
        }
    }*/
    
    void OnMouseDrag()
    {
        Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -4);
        Vector3 objPosition = _mainCamera.ScreenToWorldPoint(mousePosition);
 
        _playerTransform.position = new Vector3(Mathf.Clamp(objPosition.x,-2, 2), _playerTransform.position.y, _playerTransform.position.z);
    }
}
