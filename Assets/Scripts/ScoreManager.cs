using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public List<ThoughtsAttributesScriptableObject> thoughtsAttributes;
    private float _score;
    
    void Awake()
    {
        _score = 0;
    }

    public void Score(Color color)
    {
        for (int i = 0; i < thoughtsAttributes.Count; i++)
        {
            if (color == thoughtsAttributes[i].color)
            {
                _score += thoughtsAttributes[i].value;
                return;
            }
        }
    }
}
