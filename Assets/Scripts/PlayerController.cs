using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;


public class PlayerController : MonoBehaviour
{
    [Tooltip("SpriteRenderer component of the body sprite section")]
    public SpriteRenderer bodySprite;
    private Controls _playerControls;
    private Transform _playerTransform;
    private GameManager _gameManager;
    private Camera _mainCamera;
    private TouchState _touch;

    void Start()
    {
        _touch = new TouchState();

        _playerTransform = GetComponent<Transform>();

        _gameManager = FindObjectOfType<GameManager>();

        _mainCamera = FindObjectOfType<Camera>();
        
        //Sets the player yAxis position based on the screen size
        _playerTransform.position = new Vector3(_playerTransform.position.x, -_gameManager.ScreenBordersCoords.y-0.7f, _playerTransform.position.z);
    }

    //OnMoveGesture input event called function
    public void OnMoveGesture(InputAction.CallbackContext context)
    {
        _touch = context.ReadValue<TouchState>();
        MovePlayer();
    }
    
    void MovePlayer()
    {
        //Sets the touch position on pixels given by argument and moves player
        Vector3 touchPositionOnScreen = new Vector3(_touch.position.x, _touch.position.y, 0);
        
        //Convert touch/mouse position from pixels coordinates to world coordinates
        Vector3 touchPositionOnWorld = _mainCamera.ScreenToWorldPoint(touchPositionOnScreen);
        touchPositionOnWorld = new Vector3(touchPositionOnWorld.x, touchPositionOnWorld.y, 0);
        
        //Check if the touch/mouse position is inside the player body sprite bounds and if it's true move the player to the touch/mouse position on the X axis within the screen borders
        if (bodySprite.bounds.Contains(touchPositionOnWorld))
        {
            _playerTransform.position = new Vector3(Mathf.Clamp(touchPositionOnWorld.x,-_gameManager.ScreenBordersCoords.x+0.5f,_gameManager.ScreenBordersCoords.x-0.5f), -_gameManager.ScreenBordersCoords.y-0.7f, _playerTransform.position.z);
        }
    }
    
    
    private void OnDisable()
    {
        //Unregisters the player movement function on the input action event and disables the action when this script is disabled
        _playerControls.Player.MoveGesture.performed -= context => OnMoveGesture(context);
        _playerControls.Disable();
        _playerControls.Dispose();
    }

    private void OnEnable()
    {
        //Registers the player movement function on the input action event and enables the action when this script is enabled
        _playerControls = new Controls();
        _playerControls.Player.MoveGesture.performed += context => OnMoveGesture(context);
        _playerControls.Enable();
    }
}
