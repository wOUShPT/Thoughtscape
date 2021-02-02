using System;
using System.Collections;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

public class ThoughtDefaultBehaviour : ThoughtBehaviour
{
    public override void ResetBehaviour()
    {
        thoughtController.category = thoughtController.currentThoughtAttributes.category;
        /*if (_thoughtController.category == "Surprise")
        {
            //StartCoroutine(Surprise());
            _pulseAnimation.animate = _thoughtController.currentThoughtAttributes.animate;
            _pulseAnimation.colorPulseTime = _thoughtController.currentThoughtAttributes.animationCycleTime;
            _randomTimeInterval = Random.Range(minHorizontalForceTriggerTimeInterval, maxHorizontalForceTriggerTimeInterval);
            _rb.velocity = Vector2.zero;
            _rb.angularVelocity = 0;
            StartCoroutine(_thoughtController.HorizontalMovement());
            return;
        }*/
        thoughtController.scoreValue = thoughtController.currentThoughtAttributes.defaultValue;
        thoughtController.textColor = thoughtController.currentThoughtAttributes.textColor;
        thoughtController.outerColor = thoughtController.currentThoughtAttributes.outerColor;
        thoughtController.textMeshPro.fontMaterial.SetColor(ShaderUtilities.ID_FaceColor, thoughtController.textColor);
        thoughtController.textMeshPro.fontMaterial.SetColor(ShaderUtilities.ID_UnderlayColor, thoughtController.outerColor);
        thoughtController.textMeshPro.fontMaterial.SetColor("_DissolveEdgeColor", thoughtController.outerColor);
        randomIndex = Random.Range(0, thoughtController.currentThoughtAttributes.thoughts.Count);
        thoughtController.thoughtString = thoughtController.currentThoughtAttributes.thoughts[randomIndex];
        thoughtController.textMeshPro.text = thoughtController.thoughtString;
        pulseAnimation.animate = thoughtController.currentThoughtAttributes.animate;
        pulseAnimation.colorPulseTime = thoughtController.currentThoughtAttributes.animationCycleTime;
        thoughtController.textMeshPro.ForceMeshUpdate();
        thoughtController.textCollider.enabled = true;
        thoughtController.textCollider.offset = Vector2.zero;
        thoughtController.textCollider.size = new Vector2(thoughtController.textMeshPro.GetRenderedValues(true).x, thoughtController.textMeshPro.GetRenderedValues(true).y);
        thoughtController.horizontalForceTriggerRandomTimeInterval = Random.Range(thoughtController.minHorizontalForceTriggerTimeInterval, thoughtController.maxHorizontalForceTriggerTimeInterval);
        thoughtController.rb.velocity = Vector2.zero;
        thoughtController.rb.angularVelocity = 0;
        StartCoroutine(thoughtController.HorizontalMovement());
    }

    
}
