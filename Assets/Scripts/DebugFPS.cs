using System;
using UnityEngine;
using System.Collections;

public class DebugFPS : MonoBehaviour
{

    private GUIStyle _style;
    private string _label;
    private float _count;

    private void Awake()
    {
        _style = new GUIStyle();
       _style.normal.textColor = Color.black;
        _style.fontSize = 25;
        _style.alignment = TextAnchor.MiddleCenter;
    }

    IEnumerator Start ()
    {
        
        GUI.depth = 99;
        while (true) {
            if (Time.timeScale == 1) {
                yield return new WaitForSeconds (0.1f);
                _count = (1 / Time.deltaTime);
                _label = "FPS: " + (Mathf.Round (_count));
            } else {
                _label = "Pause";
            }
            yield return new WaitForSeconds (0.5f);
        }
    }
	
    void OnGUI ()
    {
        GUI.Label (new Rect (ScreenProperties.currentScreenCoords.xMin - 20, ScreenProperties.currentScreenCoords.yMax, 200, 50), _label, _style);
    }
}