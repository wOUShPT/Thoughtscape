using UnityEngine;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{
    [Tooltip("SpriteRenderer component of the body sprite section")]
    public SpriteRenderer bodySprite;
   
    private Transform _playerTransform;
    private Camera _mainCamera;
    private Vector3 _screenBordersCoords;

    void Awake()
    {
        _mainCamera = FindObjectOfType<Camera>();
        _playerTransform = GetComponent<Transform>();

        //Get screen size width and height in pixels and convert to world units
        _screenBordersCoords = _mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        
        //Sets the player yAxis position based on the screen size
        _playerTransform.position = new Vector3(_playerTransform.position.x, -_screenBordersCoords.y-0.7f, _playerTransform.position.z);

        //ignores collision between the player sides collider Layer 8 (Player) and Layer 9 (Thoughts)
        Physics2D.IgnoreLayerCollision(8, 9, true);
    }
    
    void Update()
    {
        //Get touch/mouse position on pixel coordinates
        Vector3 touchPositionOnScreen = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        //Convert touch/mouse position from pixels coordinates to world coordinates
        Vector3 touchPositionOnWorld = _mainCamera.ScreenToWorldPoint(touchPositionOnScreen);
        touchPositionOnWorld = new Vector3(touchPositionOnWorld.x, touchPositionOnWorld.y, bodySprite.transform.position.z);
        
        //Check if the touch/mouse position is inside the player body sprite bounds and if it's true move the player to the touch/mouse position on the X axis limited by the screen borders 
        if (bodySprite.bounds.Contains(touchPositionOnWorld))
        {
            _playerTransform.position = new Vector3(Mathf.Clamp(touchPositionOnWorld.x,-_screenBordersCoords.x+0.5f,_screenBordersCoords.x-0.5f), _playerTransform.position.y, _playerTransform.position.z);
        }
    }
}
