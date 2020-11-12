using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class WaveBehaviour : MonoBehaviour
{
    private LineRenderer _line;
    public int pointCount;
    [Range(0, 10)]
    public float waveWidth;
    public Vector3[] points;
    private Vector2[] _points2D;
    public float frequency;
    public float amplitude;
    public float speed;

    private void Awake()
    {
        _line = GetComponent<LineRenderer>();
        
        _line.positionCount = pointCount;

        _line.widthMultiplier = waveWidth;
        
        _line.useWorldSpace = false;

        points = new Vector3[pointCount];
        
        _points2D = new Vector2[pointCount];

    }

    private void Update()
    {
        for (int i = 0; i < points.Length; i++)
        {
            float x = i * frequency / points.Length;
            x += Time.time * speed;
            float y = amplitude * Mathf.Sin(x);
            points[i] = new Vector3(i - (points.Length / 2), y, 0);
            _points2D[i] = new Vector2(i - (points.Length / 2), y);
        }

        _line.SetPositions(points);
    }
}