using System;
using System.Collections;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

[Serializable]
public class ThoughtInsecurityBehaviour : ThoughtBehaviour
{
    public ThoughtsAttributesScriptableObject positiveInsecurityThoughtAttributes;
    
    public ThoughtsAttributesScriptableObject negativeInsecurityThoughtAttributes;
    public override void ResetBehaviour()
    {
        StartCoroutine(SwitchOvertime());
        pulseAnimation.animate = thoughtController.currentThoughtAttributes.animate;
        pulseAnimation.colorPulseTime = thoughtController.currentThoughtAttributes.animationCycleTime;
        thoughtController.rigidBody2D.velocity = Vector2.zero;
        thoughtController.rigidBody2D.angularVelocity = 0;
        thoughtController.horizontalForceTriggerRandomTimeInterval = Random.Range(thoughtController.minHorizontalForceTriggerTimeInterval, thoughtController.maxHorizontalForceTriggerTimeInterval);
        StartCoroutine(thoughtController.HorizontalMovement());
    }

     IEnumerator SwitchOvertime()
    {
        thoughtController.category = thoughtController.currentThoughtAttributes.category;
        randomIndex = Random.Range(0, positiveInsecurityThoughtAttributes.thoughts.Count);
        yield return new WaitForSeconds(Random.Range(0,0.2f));
        while (true)
        {
            thoughtController.scoreValue = positiveInsecurityThoughtAttributes.defaultValue;
            thoughtController.textColor = positiveInsecurityThoughtAttributes.textColor;
            thoughtController.outerColor = positiveInsecurityThoughtAttributes.outerColor;
            thoughtController.textMeshPro.fontMaterial.SetColor(ShaderUtilities.ID_FaceColor, thoughtController.textColor);
            thoughtController.textMeshPro.fontMaterial.SetColor(ShaderUtilities.ID_UnderlayColor, thoughtController.outerColor);
            thoughtController.textMeshPro.fontMaterial.SetColor("_DissolveEdgeColor", thoughtController.outerColor);
            thoughtController.thoughtString = positiveInsecurityThoughtAttributes.thoughts[randomIndex];
            thoughtController.textMeshPro.text = thoughtController.thoughtString;
            thoughtController.textMeshPro.ForceMeshUpdate();
            thoughtController.textCollider.enabled = true;
            thoughtController.textCollider.offset = Vector2.zero;
            thoughtController.textCollider.size = new Vector2(thoughtController.textMeshPro.GetRenderedValues(true).x, thoughtController.textMeshPro.GetRenderedValues(true).y);
            yield return new WaitForSeconds(thoughtController.currentThoughtAttributes.animationCycleTime);
            thoughtController.scoreValue = negativeInsecurityThoughtAttributes.defaultValue;
            thoughtController.textColor = negativeInsecurityThoughtAttributes.textColor;
            thoughtController.outerColor = negativeInsecurityThoughtAttributes.outerColor;
            thoughtController.textMeshPro.fontMaterial.SetColor(ShaderUtilities.ID_FaceColor, thoughtController.textColor);
            thoughtController.textMeshPro.fontMaterial.SetColor(ShaderUtilities.ID_UnderlayColor, thoughtController.outerColor);
            thoughtController.textMeshPro.fontMaterial.SetColor("_DissolveEdgeColor", thoughtController.outerColor);
            thoughtController.thoughtString = negativeInsecurityThoughtAttributes.thoughts[randomIndex];
            thoughtController.textMeshPro.text = thoughtController.thoughtString;
            thoughtController.textMeshPro.ForceMeshUpdate();
            thoughtController.enabled = true;
            thoughtController.textCollider.offset = Vector2.zero;
            thoughtController.textCollider.size = new Vector2(thoughtController.textMeshPro.GetRenderedValues(true).x, thoughtController.textMeshPro.GetRenderedValues(true).y);
            yield return new WaitForSeconds(thoughtController.currentThoughtAttributes.animationCycleTime);
            yield return null;
        }
    }
}
