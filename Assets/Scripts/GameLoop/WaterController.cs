using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterController : MonoBehaviour
{
    [Space(15, order = 0)]
    [Tooltip("Transform component of water wave gameObject")]
    public Transform waterWaveTransform;

    private Vector3 _waterLevelDefaultPosition;

    [Tooltip("Water level rise speed in units/second")]
    public float waterLevelRiseSpeed;

    [Tooltip("Water level drop speed in units/second")]
    public float waterLevelDropSpeed;

    public bool canWaterRise;
    public bool canWaterLow;

    private GameController _gameController;
    void Start()
    {
        _gameController = FindObjectOfType<GameController>();
        canWaterLow = false;
        canWaterRise = false;
        _waterLevelDefaultPosition = waterWaveTransform.position;
    }

    
    void Update()
    {
        if (canWaterLow)
        {
            waterWaveTransform.position = new Vector3(waterWaveTransform.position.x,
                Mathf.Clamp(waterWaveTransform.position.y - waterLevelDropSpeed * Time.deltaTime,
                    _waterLevelDefaultPosition.y, ScreenProperties.currentScreenCoords.yMin+ 0.8f)
                , waterWaveTransform.position.z);
        }

        if (canWaterRise)
        {
            waterWaveTransform.position = new Vector3(waterWaveTransform.position.x,
                Mathf.Clamp(waterWaveTransform.position.y + waterLevelRiseSpeed * Time.deltaTime,
                    _waterLevelDefaultPosition.y, ScreenProperties.currentScreenCoords.yMin + 0.8f)
                , waterWaveTransform.position.z);
        }
    }

    public void ResetWaterLevel()
    {
        waterWaveTransform.position =                                                                                                   
            new Vector3(waterWaveTransform.position.x, _waterLevelDefaultPosition.y, waterWaveTransform.position.z);
    }
}
