using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenProperties : MonoBehaviour
{
    private Camera _mainCamera;
    public static ScreenCoords currentScreenCoords;
    void Awake()
    {
        _mainCamera = FindObjectOfType<Camera>();
        Vector3 screenToWorldPointCoords = _mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        currentScreenCoords.xMin = -screenToWorldPointCoords.x;
        currentScreenCoords.xMax = screenToWorldPointCoords.x;
        currentScreenCoords.yMin = -screenToWorldPointCoords.y;
        currentScreenCoords.yMax = screenToWorldPointCoords.y;

    }

    
    public struct ScreenCoords
    {
        public float xMin;
        public float xMax;
        public float yMin;
        public float yMax;
    }
}
