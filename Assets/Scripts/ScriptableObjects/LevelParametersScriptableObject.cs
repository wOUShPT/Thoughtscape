using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newLevelParameters", menuName = "LevelParameters", order = 2)]
public class LevelParametersScriptableObject : ScriptableObject
{
    public string day;
    public float scoreBaseTime;
    public float meterCenterSpreadValue;
    public float horizontalForceTriggerTime;
    public float horizontalForceIncrement;
    public float meterBaseMoveSpeed;
    public float meterLimitsTimeToDeath;
    public float waterLevelRiseSpeed;
    public float waterLevelDropSpeed;
    public float minTimeBetweenSpawns;
    public float maxTimeBetweenSpawns;
    public List<ThoughtSpawnRate> thoughtSpawnPropertiesList;

    [Serializable]
    public struct ThoughtSpawnRate
    {
        public string category;
        public bool hasHorizontalForce;
        public float dropSpeed;
        public float centerMeterZoneSpawnRatePercentage;
        public float positiveZoneMeterSpawnRatePercentage;
        public float negativeZoneMeterSpawnRatePercentage;
        public float limitZoneMeterSpawnRatePercentage;
    }
}
