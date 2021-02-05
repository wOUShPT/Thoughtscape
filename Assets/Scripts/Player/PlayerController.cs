using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;


public class PlayerController : MonoBehaviour
{
    [Tooltip("Player touch detection collider component")]
    public BoxCollider2D touchDrag2DCollider;
    private InputManager _inputManager;
    private Transform _playerTransform;
    private UIController _uiController;
    private Vector2 _touchPosition;

    void Start()
    {
        _inputManager = FindObjectOfType<InputManager>();
        
        //Registers the player movement function on the input action event and enables the action when this script is enabled
        _inputManager.onStartTouchEvent.AddListener(OnTouch);
        
        _touchPosition = Vector2.zero;

        _playerTransform = GetComponent<Transform>();

        _uiController = FindObjectOfType<UIController>();

        //Sets the player yAxis position based on the screen size
        _playerTransform.position = new Vector3(_playerTransform.position.x, ScreenProperties.currentScreenCoords.yMin-0.25f, 0);
    }
    
    private void OnDisable()
    {
        //Unregisters the player movement function on the input action event and disables the action when this script is disabled
        _inputManager.onStartTouchEvent.RemoveListener(OnTouch);
    }
    
    void MovePlayer()
    {
        Vector3 touchPositionOnWorld = new Vector3(_touchPosition.x, _touchPosition.y, 0);
        
        //Check if the touch/mouse position is inside the player body sprite bounds and if it's true move the player to the touch/mouse position on the X axis within the screen borders
        if (touchDrag2DCollider.bounds.Contains(touchPositionOnWorld))
        {
            _playerTransform.position = new Vector3(Mathf.Clamp(touchPositionOnWorld.x,ScreenProperties.currentScreenCoords.xMin+0.5f,ScreenProperties.currentScreenCoords.xMax-0.5f), ScreenProperties.currentScreenCoords.yMin-0.25f, _playerTransform.position.z);
        }
    }

    private void OnTouch(TouchPhase phase, Vector2 position, float time)
    {
        _touchPosition = position;
        if (!_uiController.optionsMenu.isToggled)
        {
            MovePlayer();
        }
        MovePlayer();
    }
}
