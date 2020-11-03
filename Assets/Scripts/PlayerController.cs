using UnityEngine;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{
    [Tooltip("SpriteRenderer component of the body sprite section")]
    public SpriteRenderer bodySprite;
   
    private Transform _playerTransform;
    private Camera _mainCamera;

    void Awake()
    {
        _playerTransform = GetComponent<Transform>();
        _mainCamera = FindObjectOfType<Camera>();
        
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
        
        //Check if the touch/mouse position is inside the player body sprite bounds and if it's true move the player to the touch/mouse position on the X axis
        if (bodySprite.bounds.Contains(touchPositionOnWorld))
        { 
            _playerTransform.position = new Vector3(touchPositionOnWorld.x, _playerTransform.position.y, _playerTransform.position.z);
        }
    }
}
