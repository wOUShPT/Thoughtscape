using System.Collections;
using System.Collections.Generic;
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
        float colorStep = 1f;
        while (colorStep > 0)
        {
            colorStep -= _fadeSpeed * Time.deltaTime;
            _text.fontMaterial.SetColor(ShaderUtilities.ID_FaceColor, new Color(textcolor.r, textcolor.g, textcolor.b, colorStep));
            _text.fontMaterial.SetColor(ShaderUtilities.ID_UnderlayColor, new Color(outerColor.r, outerColor.g, outerColor.b, colorStep));
            yield return null;
        }
        gameObject.SetActive(false);
    }

    public void ResetFadeState()
    {
        for (int i = 0; i < _text.mesh.colors.Length; i++)
        {
            _text.mesh.colors[i] = Color.white;
        }
    }
}
