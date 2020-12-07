using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

public class ThoughtDoubtBehaviour : ThoughtBehaviour
{
    private ThoughtDoubtBehaviour _doubtBehaviour;
    private TextMeshPro _text;
    private Transform _currentTransform;
    private Rigidbody2D _rb;
    private BoxCollider2D _collider;
    private int _randomIndex;
    private PulseAnimation _pulseAnimation;
    public ThoughtsAttributesScriptableObject positiveThoughtAttributes;
    public ThoughtsAttributesScriptableObject negativeThoughtAttributes;

    public override void Awake()
    {
        base.Awake();
        _text = GetComponentInChildren<TextMeshPro>();
        _currentTransform = GetComponent<Transform>();
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<BoxCollider2D>();
        _pulseAnimation = GetComponent<PulseAnimation>();
    }

    public override void ResetBehaviour()
    {
        category = thoughtsAttributesList[currentIndex].category;
        _randomIndex = Random.Range(0, thoughtsAttributesList[currentIndex].thoughts.Count);
        StartCoroutine(Intermittence());
        _pulseAnimation.animate = thoughtsAttributesList[currentIndex].animate;
        _pulseAnimation.colorPulseTime = thoughtsAttributesList[currentIndex].animationCycleTime;
        _text.ForceMeshUpdate();
        _currentTransform.localRotation = Quaternion.identity;
        _rb.velocity = Vector2.zero;
        _rb.angularVelocity = 0;
        StartCoroutine(HorizontalMovement());
    }

    IEnumerator Intermittence()
    {
        while (true)
        {
            scoreValue = positiveThoughtAttributes.defaultValue;
            textColor = positiveThoughtAttributes.textColor;
            outerColor = positiveThoughtAttributes.outerColor;
            _text.fontMaterial.SetColor(ShaderUtilities.ID_FaceColor, textColor);
            _text.fontMaterial.SetColor(ShaderUtilities.ID_UnderlayColor, outerColor);
            _text.fontMaterial.SetColor("_DissolveEdgeColor", outerColor);
            thoughtString = thoughtsAttributesList[_randomIndex].thoughts[_randomIndex];
            _text.text = thoughtString;
            _text.ForceMeshUpdate();
            _collider.enabled = true;
            _collider.offset = Vector2.zero;
            _collider.size = new Vector2(_text.GetRenderedValues(true).x, _text.GetRenderedValues(true).y);
            yield return new WaitForSeconds(1);
            scoreValue = negativeThoughtAttributes.defaultValue;
            textColor = negativeThoughtAttributes.textColor;
            outerColor = negativeThoughtAttributes.outerColor;
            _text.fontMaterial.SetColor(ShaderUtilities.ID_FaceColor, textColor);
            _text.fontMaterial.SetColor(ShaderUtilities.ID_UnderlayColor, outerColor);
            _text.fontMaterial.SetColor("_DissolveEdgeColor", outerColor);
            thoughtString = thoughtsAttributesList[_randomIndex].thoughts[_randomIndex];
            _text.text = thoughtString;
            _text.ForceMeshUpdate();
            _collider.enabled = true;
            _collider.offset = Vector2.zero;
            _collider.size = new Vector2(_text.GetRenderedValues(true).x, _text.GetRenderedValues(true).y);
            yield return new WaitForSeconds(1);
            yield return null;
        }
    }
}
