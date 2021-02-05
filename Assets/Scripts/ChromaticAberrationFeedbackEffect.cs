using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ChromaticAberrationFeedbackEffect : MonoBehaviour
{
    [Tooltip("Post Processing Volume component")]
    public Volume postProcessingVolume;
    private ChromaticAberration _chromaticAberration;
    private int _chromaticAberrationSign;
    [Tooltip("Time duration of the catch thoughts chromatic aberration feedback")]
    public float chromaticAberrationFeedbackEffectTime;
    
    void Start()
    {
        postProcessingVolume.profile.TryGet(out _chromaticAberration);
        _chromaticAberration.intensity.min = 0f;
        _chromaticAberration.intensity.max = 1f;
        _chromaticAberrationSign = 1;
    }
    
    
    private IEnumerator ChromaticAberrationFeedback()
    {
        float speed = (_chromaticAberration.intensity.max - _chromaticAberration.intensity.min) / (chromaticAberrationFeedbackEffectTime/2);
        while (_chromaticAberration.intensity.value <= _chromaticAberration.intensity.max)
        {
            _chromaticAberration.intensity.value += _chromaticAberrationSign * speed * Time.deltaTime;
            _chromaticAberration.intensity.value = Mathf.Clamp(_chromaticAberration.intensity.value, _chromaticAberration.intensity.min, _chromaticAberration.intensity.max);
            if (_chromaticAberration.intensity.value == _chromaticAberration.intensity.max)
            {
                _chromaticAberrationSign = -_chromaticAberrationSign;
            }

            if (_chromaticAberration.intensity.value == _chromaticAberration.intensity.min)
            {
                _chromaticAberrationSign = -_chromaticAberrationSign;
                break;
            }
            yield return null;
        }
    }

    public void Feedback()
    {
        StartCoroutine(ChromaticAberrationFeedback());
    }
    
}
