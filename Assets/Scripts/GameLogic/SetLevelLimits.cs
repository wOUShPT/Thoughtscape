using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetLevelLimits : MonoBehaviour
{
    [Space(30, order = 0)]
    [Header("BoxCollider2D Left and Right Components", order = 1)]
    [Space(15, order = 2)]
    
    [Tooltip("Left limit BoxCollider2D Component")]
    public BoxCollider2D leftLimitCollider;
    [Tooltip("Right limit BoxCollider2D Component")]
    public BoxCollider2D rightLimitCollider;
    
    private Camera _mainCamera;
    private Vector3 _screenBordersCoords;
    
    //Set the level limits left and right collider (thoughts collision control)
    void Awake()
    {
        _mainCamera = FindObjectOfType<Camera>();
        _screenBordersCoords = _mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        leftLimitCollider.offset = new Vector2(-_screenBordersCoords.x-1f, leftLimitCollider.offset.y);
        rightLimitCollider.offset = new Vector2(_screenBordersCoords.x+1f, rightLimitCollider.offset.y);
    }
}
