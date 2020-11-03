using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newThoughtData", menuName = "ThoughtAttribute", order = 1)]
public class ThoughtsAttributesScriptableObject : ScriptableObject
{
    public string category = "Default";
    public float value = 0;
    public Color color = Color.black;
    public List<string> thoughts;
}
