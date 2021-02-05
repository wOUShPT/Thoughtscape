using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class UIFallAnimation : MonoBehaviour
{
    private Rigidbody2D _rigidBody2D;
    void Awake()
    {
        _rigidBody2D = GetComponent<Rigidbody2D>();
        _rigidBody2D.gravityScale = 0;
    }

    public void Animation(float dropTime)
    {
        float speed = (transform.position.y - ScreenProperties.currentScreenCoords.yMin) * dropTime;
        _rigidBody2D.velocity = Vector2.down * speed;
    }
}
