using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class WaveBehaviour : MonoBehaviour
{
    
    [Tooltip("Number of points used on the linerenderer")]
    public int pointCount;
    
    [Tooltip("Wave points coords array")]
    public Vector3[] points;
    
    [Tooltip("Width of the linerenderer")]
    [Range(0, 10)]
    public float waveWidth;
    
    [Tooltip("Frequency value of the wave")]
    public float frequency;
    
    [Tooltip("Amplitude value of the wave")]
    public float amplitude;
    
    [Tooltip("Move speed value of the wave in units/second")]
    public float moveSpeed;
    
    private LineRenderer _lineRenderer;


    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = pointCount;
        _lineRenderer.widthMultiplier = waveWidth;
        _lineRenderer.useWorldSpace = false;

        //Set the wave points array based on the given number
        points = new Vector3[pointCount];

    }

    private void Update()
    {
        
        //for each point on the array set an X and Y value based on a Sin equation
        for (int i = 0; i < points.Length; i++)
        {
            float x = i * frequency / points.Length;
            x += Time.time * moveSpeed;
            float y = amplitude * Mathf.Sin(x);
            points[i] = new Vector3(i - (points.Length / 2), y, 0);
        }

        _lineRenderer.SetPositions(points);
    }
}