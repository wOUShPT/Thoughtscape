using System;
using UnityEngine;
using System.Collections;
using System.Reflection.Emit;
using UnityEngine.UI;

public class DebugFPS : MonoBehaviour
{
    public Text text;
    private string _label;
    private float _count;

    
    IEnumerator Start ()
    {
        while (true) {
            if (Time.timeScale == 1) {
                yield return new WaitForSeconds (0.1f);
                _count = (1 / Time.deltaTime);
                _label = "FPS: " + (Mathf.Round (_count));
            } else {
                _label = "Pause";
            }

            text.text = _label;
            yield return new WaitForSeconds (0.5f);
        }
    }
}