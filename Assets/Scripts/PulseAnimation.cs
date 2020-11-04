using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PulseAnimation : MonoBehaviour
{
    [Space(30, order = 0)]
    [Header("TextMeshPro Component in Thought Prefab", order = 1)]
    [Space(15, order = 2)]
    
    [Tooltip("gameobject with TextMeshPro attached")]
    public TextMeshPro text;
    
    [Space(30, order = 3)]
    [Header("Pulse Time and Enable/Disable Animation", order = 4)]
    [Space(15, order = 5)]
    
    [Tooltip("Time between pulses in seconds")]
    public float colorPulseTime;
    
    [Tooltip("Enable/disable animation bool")]
    public bool animate;
    
    private float _colorPulseSpeed;
    private float _currentDilatationValue;
    private int _colorDilatationSign;

    private void Awake()
    {
        //Sets default animation related values and calculates the color pulse speed based on the given color pulse time
        //the "4" value below is the delta on the font shader Underlay Dilate property (Glow similar effect)
        //the Underlay Dilate property varies between the values of -1 and 1, so it goes from the default -1 to 1 and turns back -1 which equals a delta of 4
        _colorPulseSpeed = 4 / colorPulseTime;
        _colorDilatationSign = 1;
        _currentDilatationValue = -1;
    }

    void Update()
    {
        //Checks if the animate bool is true or false / Runs the color pulse animation or sets it to static color
        if (animate)
        {
            AnimateColorPulse();
        }
        else
        {
            _currentDilatationValue = 1;
        }
    }
    
    
    //Color pulse animation
    void AnimateColorPulse()
    {
        //Calculates the increment of the Underlay Dilation property through time and locks it between the allowed min and max values
        _currentDilatationValue += _colorDilatationSign * _colorPulseSpeed * Time.deltaTime;
        _currentDilatationValue = Mathf.Clamp(_currentDilatationValue, -1, 1);
        
        //Sets the Underlay Dilation property value
        //text.fontMaterial.SetFloat(ShaderUtilities.ID_UnderlayDilate, _currentDilatationValue);
        text.fontMaterial.SetFloat(ShaderUtilities.ID_UnderlayDilate, _currentDilatationValue);
        
        
        //Switch the increment sign on min and max values
        if (_currentDilatationValue == -1)
        {
            _colorDilatationSign = 1;
        }

        if (_currentDilatationValue == 1)
        {
            _colorDilatationSign = -1;
        }
    }

    
    //Sets the default animation values when the object its activated
    private void OnEnable()
    {
        _colorDilatationSign = 1;
        _currentDilatationValue = -1;
    }
}
