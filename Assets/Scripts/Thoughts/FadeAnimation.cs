using System;
using System.Collections;
using System.Collections.Generic;
using AdvancedDissolve_Example;
using TMPro;
using UnityEngine;

public class FadeAnimation : MonoBehaviour
{
    public float fadeTimeDuration;
    private float _fadeSpeed;
    private TextMeshPro _text;

    void Awake()
    {
        _text = GetComponentInChildren<TextMeshPro>();
        _fadeSpeed = 1 / fadeTimeDuration;
    }

    public IEnumerator AnimateFade(Color textcolor, Color outerColor)
    {
        float cutOutStep = 0f;
        while (cutOutStep < 1)
        {
            cutOutStep += _fadeSpeed * Time.deltaTime;
            _text.fontMaterial.SetFloat("_DissolveCutoff", cutOutStep);
            yield return null;
        }
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        ResetFadeState();
    }

    public void ResetFadeState()
    {
        for (int i = 0; i < _text.mesh.colors.Length; i++)
        {
            _text.fontMaterial.SetFloat("_DissolveCutoff",0);
            _text.mesh.colors[i] = Color.white;
        }
    }
}
