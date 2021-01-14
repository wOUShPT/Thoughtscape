using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private Controls _controls;
    public TouchEvent onStartTouch;
    public TouchEvent onEndTouch;
    public Camera mainCamera;
    void Awake()
    {
        _controls = new Controls();
        onStartTouch = new TouchEvent();
        onEndTouch = new TouchEvent();
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        _controls.Enable();
    }

    private void OnDisable()
    {
        _controls.Disable();
    }

    private void Start()
    {
        _controls.Touch.PrimaryContact.started += ctx => StartTouchPrimary(ctx);
        _controls.Touch.PrimaryContact.canceled += ctx => EndTouchPrimary(ctx);
    }

    void StartTouchPrimary(InputAction.CallbackContext context)
    {
        if (onStartTouch != null)
        {
            //onStartTouch.Invoke(ScreenProperties.ScreenToWorldPosition(mainCamera, _controls.Touch.PrimaryPosition.ReadValue<Vector2>()), (float)context.startTime);
        }
    }
    
    void EndTouchPrimary(InputAction.CallbackContext context)
    {
        if (onEndTouch != null)
        {
            //onStartTouch.Invoke(ScreenProperties.ScreenToWorldPosition(mainCamera, _controls.Touch.PrimaryPosition.ReadValue<Vector2>()), (float)context.startTime);
        }
    }
    
    
    public class TouchEvent : UnityEvent<Vector2, float>
    {
    
    }
}
