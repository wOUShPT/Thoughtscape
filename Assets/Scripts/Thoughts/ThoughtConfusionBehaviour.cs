using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ThoughtConfusionBehaviour : ThoughtBehaviour
{
    public List<ThoughtsAttributesScriptableObject> thoughtsAttributesList;

    public override void ResetBehaviour()
    {
        StartCoroutine(Confusion());
        thoughtController.rigidBody2D.velocity = Vector2.zero;
        thoughtController.rigidBody2D.angularVelocity = 0;
        thoughtController.horizontalForceTriggerRandomTimeInterval = Random.Range(thoughtController.minHorizontalForceTriggerTimeInterval, thoughtController.maxHorizontalForceTriggerTimeInterval);
        StartCoroutine(thoughtController.HorizontalMovement());
    }
    
    IEnumerator Confusion()
    {
        thoughtController.textColor = thoughtController.currentThoughtAttributes.textColor;
        thoughtController.outerColor = thoughtController.currentThoughtAttributes.outerColor;
        thoughtController.textMeshPro.fontMaterial.SetColor(ShaderUtilities.ID_FaceColor, thoughtController.textColor);
        thoughtController.textMeshPro.fontMaterial.SetColor(ShaderUtilities.ID_UnderlayColor, thoughtController.outerColor);
        thoughtController.textMeshPro.fontMaterial.SetColor("_DissolveEdgeColor", thoughtController.outerColor);
        randomIndex = Random.Range(0, thoughtController.currentThoughtAttributes.thoughts.Count);
        thoughtController.thoughtString = thoughtController.currentThoughtAttributes.thoughts[randomIndex];
        thoughtController.textMeshPro.text = thoughtController.thoughtString;
        thoughtController.textMeshPro.ForceMeshUpdate();
        thoughtController.textCollider.enabled = false;
        thoughtController.textCollider.offset = Vector2.zero;
        thoughtController.textCollider.size = new Vector2(thoughtController.textMeshPro.GetRenderedValues(true).x, thoughtController.textMeshPro.GetRenderedValues(true).y);
        float timer = 0;
        while (transform.position.y > 0)
        {
            timer += Time.deltaTime;
            if (timer > 0.1f)
            {
                timer = 0;
                randomIndex = Random.Range(0, thoughtController.currentThoughtAttributes.thoughts.Count);
                thoughtController.thoughtString = thoughtController.currentThoughtAttributes.thoughts[randomIndex];
                thoughtController.textMeshPro.text = thoughtController.thoughtString;
            }
            yield return null;
        }
        yield return new WaitUntil(() => transform.position.y < 0);
        int index = Random.Range(0, thoughtsAttributesList.Count);
        thoughtController.scoreValue = thoughtsAttributesList[index].defaultValue;
        randomIndex = Random.Range(0, thoughtsAttributesList[index].thoughts.Count);
        thoughtController.thoughtString = thoughtsAttributesList[index].thoughts[randomIndex];
        thoughtController.textMeshPro.text = thoughtController.thoughtString;
        thoughtController.textColor = thoughtsAttributesList[index].textColor;
        thoughtController.outerColor = thoughtsAttributesList[index].outerColor;
        thoughtController.textMeshPro.fontMaterial.SetColor(ShaderUtilities.ID_FaceColor, thoughtController.textColor);
        thoughtController.textMeshPro.fontMaterial.SetColor(ShaderUtilities.ID_UnderlayColor, thoughtController.outerColor);
        thoughtController.textMeshPro.fontMaterial.SetColor("_DissolveEdgeColor", thoughtController.outerColor);
        thoughtController.textMeshPro.ForceMeshUpdate();
        thoughtController.textCollider.enabled = true;
        thoughtController.textCollider.offset = Vector2.zero;
        thoughtController.textCollider.size = new Vector2(thoughtController.textMeshPro.GetRenderedValues(true).x, thoughtController.textMeshPro.GetRenderedValues(true).y);
        pulseAnimation.animate = thoughtsAttributesList[index].animate;
        pulseAnimation.colorPulseTime = thoughtsAttributesList[index].animationCycleTime;
    }
    
}
