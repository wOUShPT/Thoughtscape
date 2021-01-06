using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class UIFallAnimation : MonoBehaviour
{
    private Rigidbody2D _rb;
    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.gravityScale = 0;
    }

    public void Animation(float dropTime)
    {
        float speed = (transform.position.y - ScreenProperties.currentScreenCoords.yMin) * dropTime;
        _rb.velocity = Vector2.down * speed;
    }
}
