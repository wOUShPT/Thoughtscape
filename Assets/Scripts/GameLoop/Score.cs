using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newScore", menuName = "ScoreData", order = 3)]
public class Score : ScriptableObject
{
    public int lastScore;
    public int bestScore;
}
