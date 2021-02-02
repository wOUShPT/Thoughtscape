using System;
using System.Collections;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

[Serializable]
public class ThoughtInsecurityBehaviour : ThoughtBehaviour
{
    public ThoughtsAttributesScriptableObject positiveDoubtThoughtAttributes;
    
    public ThoughtsAttributesScriptableObject negativeDoubtThoughtAttributes;
    public override void ResetBehaviour()
    {
        StartCoroutine(SwitchOvertime());
        pulseAnimation.animate = thoughtController.currentThoughtAttributes.animate;
        pulseAnimation.colorPulseTime = thoughtController.currentThoughtAttributes.animationCycleTime;
        thoughtController.horizontalForceTriggerRandomTimeInterval = Random.Range(thoughtController.minHorizontalForceTriggerTimeInterval, thoughtController.maxHorizontalForceTriggerTimeInterval);
        thoughtController.rb.velocity = Vector2.zero;
        thoughtController.rb.angularVelocity = 0;
        StartCoroutine(thoughtController.HorizontalMovement());
    }

     IEnumerator SwitchOvertime()
    {
        thoughtController.category = thoughtController.currentThoughtAttributes.category;
        randomIndex = Random.Range(0, positiveDoubtThoughtAttributes.thoughts.Count);
        yield return new WaitForSeconds(Random.Range(0,0.2f));
        while (true)
        {
            thoughtController.scoreValue = positiveDoubtThoughtAttributes.defaultValue;
            thoughtController.textColor = positiveDoubtThoughtAttributes.textColor;
            thoughtController.outerColor = positiveDoubtThoughtAttributes.outerColor;
            thoughtController.textMeshPro.fontMaterial.SetColor(ShaderUtilities.ID_FaceColor, thoughtController.textColor);
            thoughtController.textMeshPro.fontMaterial.SetColor(ShaderUtilities.ID_UnderlayColor, thoughtController.outerColor);
            thoughtController.textMeshPro.fontMaterial.SetColor("_DissolveEdgeColor", thoughtController.outerColor);
            thoughtController.thoughtString = positiveDoubtThoughtAttributes.thoughts[randomIndex];
            thoughtController.textMeshPro.text = thoughtController.thoughtString;
            thoughtController.textMeshPro.ForceMeshUpdate();
            thoughtController.textCollider.enabled = true;
            thoughtController.textCollider.offset = Vector2.zero;
            thoughtController.textCollider.size = new Vector2(thoughtController.textMeshPro.GetRenderedValues(true).x, thoughtController.textMeshPro.GetRenderedValues(true).y);
            yield return new WaitForSeconds(thoughtController.currentThoughtAttributes.animationCycleTime);
            thoughtController.scoreValue = negativeDoubtThoughtAttributes.defaultValue;
            thoughtController.textColor = negativeDoubtThoughtAttributes.textColor;
            thoughtController.outerColor = negativeDoubtThoughtAttributes.outerColor;
            thoughtController.textMeshPro.fontMaterial.SetColor(ShaderUtilities.ID_FaceColor, thoughtController.textColor);
            thoughtController.textMeshPro.fontMaterial.SetColor(ShaderUtilities.ID_UnderlayColor, thoughtController.outerColor);
            thoughtController.textMeshPro.fontMaterial.SetColor("_DissolveEdgeColor", thoughtController.outerColor);
            thoughtController.thoughtString = negativeDoubtThoughtAttributes.thoughts[randomIndex];
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
