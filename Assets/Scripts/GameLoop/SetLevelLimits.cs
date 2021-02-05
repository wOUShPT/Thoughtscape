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
    
    
    //Set the level limits left and right collider (thoughts collision control)
    void Awake()
    {
        leftLimitCollider.offset = new Vector2(ScreenProperties.currentScreenCoords.xMin-0.5f, leftLimitCollider.offset.y);
        rightLimitCollider.offset = new Vector2(ScreenProperties.currentScreenCoords.xMax+0.5f, rightLimitCollider.offset.y);
    }
}
