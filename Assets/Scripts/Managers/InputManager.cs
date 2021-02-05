using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using RDG;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class InputManager : MonoBehaviour
{
    private Controls _controls;
    public TouchEvent onStartTouchEvent;
    public TouchEvent onTouchEvent;
    public TouchEvent onEndTouchEvent;
    public bool canVibrate;
    private Camera _mainCamera;
    private void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += (arg0, mode) =>  {_mainCamera = Camera.main;};
        _controls = new Controls();
        onStartTouchEvent = new TouchEvent();
        onTouchEvent =  new TouchEvent();
        onEndTouchEvent = new TouchEvent();
        canVibrate = true;
        _controls.Enable();
        _controls.Touch.PrimaryContact.started += ctx => StartTouchPrimary(ctx);
        _controls.Touch.PrimaryContact.performed += ctx => WhileTouchPrimary(ctx);
    }

    private void OnDisable()
    {
        _controls.Touch.PrimaryContact.started -= ctx => StartTouchPrimary(ctx);
        _controls.Touch.PrimaryContact.performed -= ctx => WhileTouchPrimary(ctx);
        _controls.Disable();
        _controls.Dispose();
    }

    void StartTouchPrimary(InputAction.CallbackContext context)
    {
        if (onStartTouchEvent != null)
        {
            onStartTouchEvent.Invoke(_controls.Touch.PrimaryContact.ReadValue<TouchState>().phase, (ScreenToWorldPosition(_controls.Touch.PrimaryContact.ReadValue<TouchState>().position)), (float)context.startTime);
        }
    }
    
    void EndTouchPrimary(InputAction.CallbackContext context)
    {
        if (onEndTouchEvent != null)
        {
            onStartTouchEvent.Invoke(_controls.Touch.PrimaryContact.ReadValue<TouchState>().phase, (ScreenToWorldPosition(_controls.Touch.PrimaryContact.ReadValue<TouchState>().position)), (float)context.startTime);
        }
    }
    
    void WhileTouchPrimary(InputAction.CallbackContext context)
    {
        if (onTouchEvent != null)
        {
            onStartTouchEvent.Invoke(_controls.Touch.PrimaryContact.ReadValue<TouchState>().phase, (ScreenToWorldPosition(_controls.Touch.PrimaryContact.ReadValue<TouchState>().position)), (float)context.startTime);
        }
    }

    private Vector3 ScreenToWorldPosition(Vector3 position)
    {
        if (_mainCamera!=null)
        {
            position.z = 0;
            return _mainCamera.ScreenToWorldPoint(position);
        }
        return new Vector3(0,0,0);
    }
    
    public void ToggleVibrate(bool state)
    {
        canVibrate = state;
    }


    public void Vibrate()
    {
        if (canVibrate)
        {
            Vibration.Vibrate(250);
        }
    }
    
    public class TouchEvent : UnityEvent<TouchPhase, Vector2, float>
    {
        
    }
}
