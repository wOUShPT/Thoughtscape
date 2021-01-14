using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScreenProperties : MonoBehaviour
{
    private Camera _mainCamera;
    public static ScreenCoords currentScreenCoords;
    void Awake()
    {
        _mainCamera = FindObjectOfType<Camera>();
        Vector3 screenToWorldPointCoords = _mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        currentScreenCoords.xMin = -screenToWorldPointCoords.x *_mainCamera.rect.width;
        currentScreenCoords.xMax = screenToWorldPointCoords.x * _mainCamera.rect.width;
        currentScreenCoords.yMin = -screenToWorldPointCoords.y * _mainCamera.rect.height;
        currentScreenCoords.yMax = screenToWorldPointCoords.y * _mainCamera.rect.height;

    }

    
    public struct ScreenCoords
    {
        public float xMin;
        public float xMax;
        public float yMin;
        public float yMax;
    }
}
