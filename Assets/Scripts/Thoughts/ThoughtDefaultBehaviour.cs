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
        thoughtController.rigidBody2D.velocity = Vector2.zero;
        thoughtController.rigidBody2D.angularVelocity = 0;
        thoughtController.horizontalForceTriggerRandomTimeInterval = Random.Range(thoughtController.minHorizontalForceTriggerTimeInterval, thoughtController.maxHorizontalForceTriggerTimeInterval);
        StartCoroutine(thoughtController.HorizontalMovement());
    }

    
}
