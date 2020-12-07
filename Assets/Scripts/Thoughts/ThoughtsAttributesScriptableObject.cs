using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newThoughtData", menuName = "ThoughtAttribute", order = 1)]
public class ThoughtsAttributesScriptableObject : ScriptableObject
{
    public string category = "Default";
    public float dropSpeed;
    public float centerMeterZoneSpawnRatePercentage;
    public float goodZoneMeterSpawnRatePercentage;
    public float badZoneMeterSpawnRatePercentage;
    public float limitZoneMeterSpawnRatePercentage;
    public bool canSpawn;
    public bool hasHorizontalForce;
    public float defaultValue = 0;
    public Color textColor = Color.black;
    public Color outerColor = Color.black;
    public bool animate;
    public float animationCycleTime;
    public List<string> thoughts;
}