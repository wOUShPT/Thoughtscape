using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newThoughtData", menuName = "ThoughtAttribute", order = 1)]
public class ThoughtsAttributesScriptableObject : ScriptableObject
{
    public string category = "Default";
    public float spawnRatePercentage = 100;
    public float value = 0;
    public Color textColor = Color.black;
    public Color outerColor = Color.black;
    public List<string> thoughts;
}