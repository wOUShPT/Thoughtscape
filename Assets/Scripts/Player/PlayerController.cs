using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    [Tooltip("SpriteRenderer component of the body sprite section")]
    public BoxCollider2D touchDragCollider;
    private Controls _playerControls;
    private Transform _playerTransform;
    private PauseMenu _pauseMenu;
    private Camera _mainCamera;
    private Vector2 _touchPosition;

    void Start()
    {
        _touchPosition = Vector2.zero;

        _playerTransform = GetComponent<Transform>();

        _pauseMenu = FindObjectOfType<PauseMenu>();

        _mainCamera = FindObjectOfType<Camera>();
        
        //Sets the player yAxis position based on the screen size
        _playerTransform.position = new Vector3(_playerTransform.position.x, ScreenProperties.currentScreenCoords.yMin-0.25f, 0);
    }

    //OnMoveGesture input event called function
    public void OnMoveGesture(InputAction.CallbackContext context)
    {
        _touchPosition = context.ReadValue<Vector2>();
        if (!_pauseMenu.isPaused)
        {
            MovePlayer();
        }
    }
    
    void MovePlayer()
    {
        //Sets the touch position on pixels given by argument and moves player
        Vector3 touchPositionOnScreen = new Vector3(_touchPosition.x, _touchPosition.y, 0);
        
        //Convert touch/mouse position from pixels coordinates to world coordinates
        Vector3 touchPositionOnWorld = _mainCamera.ScreenToWorldPoint(touchPositionOnScreen);
        touchPositionOnWorld = new Vector3(touchPositionOnWorld.x, touchPositionOnWorld.y, 0);
        
        //Check if the touch/mouse position is inside the player body sprite bounds and if it's true move the player to the touch/mouse position on the X axis within the screen borders
        if (touchDragCollider.bounds.Contains(touchPositionOnWorld))
        {
            _playerTransform.position = new Vector3(Mathf.Clamp(touchPositionOnWorld.x,ScreenProperties.currentScreenCoords.xMin+0.5f,ScreenProperties.currentScreenCoords.xMax-0.5f), ScreenProperties.currentScreenCoords.yMin-0.25f, _playerTransform.position.z);
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
