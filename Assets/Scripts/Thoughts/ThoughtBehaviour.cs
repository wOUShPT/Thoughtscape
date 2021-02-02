using UnityEngine;

public abstract class ThoughtBehaviour : MonoBehaviour
{
    protected int randomIndex;
    
    protected PulseAnimation pulseAnimation;
    protected ThoughtController thoughtController;

    public void Awake()
    {
        thoughtController = GetComponent<ThoughtController>();
        pulseAnimation = GetComponent<PulseAnimation>();
    }

    //Reset the Thoughts attributes randomly, based on a given thought attributes data container (ScriptableObject) / reset other behaviour default values
    public abstract void ResetBehaviour();

    /*IEnumerator Surprise()
    {
        Debug.Log(_currentPosition.y.ToString());
        int index = Random.Range(0, thoughtsAttributesSurpriseList.Count);
        scoreValue = thoughtsAttributesSurpriseList[index].defaultValue;
        textColor = thoughtsAttributesList[currentIndex].textColor;
        outerColor = thoughtsAttributesList[currentIndex].outerColor;
        _text.fontMaterial.SetColor(ShaderUtilities.ID_FaceColor, textColor);
        _text.fontMaterial.SetColor(ShaderUtilities.ID_UnderlayColor, outerColor);
        _text.fontMaterial.SetColor("_DissolveEdgeColor", outerColor);
        Debug.Log("waiting");
        _randomIndex = Random.Range(0, thoughtsAttributesList[currentIndex].thoughts.Count);
        thoughtString = thoughtsAttributesList[currentIndex].thoughts[_randomIndex];
        _text.text = thoughtString;
        _text.ForceMeshUpdate();
        _collider.enabled = false;
        _collider.offset = Vector2.zero;
        _collider.size = new Vector2(_text.GetRenderedValues(true).x, _text.GetRenderedValues(true).y);
        float timer = 0;
        while (_currentPosition.y > 0)
        {
            timer += Time.deltaTime;
            if (timer > 0.1f)
            {
                timer = 0;
                _randomIndex = Random.Range(0, thoughtsAttributesList[currentIndex].thoughts.Count);
                thoughtString = thoughtsAttributesList[currentIndex].thoughts[_randomIndex];
                _text.text = thoughtString;
            }
            yield return null;
        }
        yield return new WaitUntil(() => _currentPosition.y < 0);
        Debug.Log("Waited");
        _randomIndex = Random.Range(0, thoughtsAttributesSurpriseList[index].thoughts.Count);
        thoughtString = thoughtsAttributesSurpriseList[index].thoughts[_randomIndex];
        _text.text = thoughtString;
        textColor = thoughtsAttributesSurpriseList[index].textColor;
        outerColor = thoughtsAttributesSurpriseList[index].outerColor;
        _text.fontMaterial.SetColor(ShaderUtilities.ID_FaceColor, textColor);
        _text.fontMaterial.SetColor(ShaderUtilities.ID_UnderlayColor, outerColor);
        _text.fontMaterial.SetColor("_DissolveEdgeColor", outerColor);
        _text.ForceMeshUpdate();
        _collider.enabled = true;
        _collider.offset = Vector2.zero;
        _collider.size = new Vector2(_text.GetRenderedValues(true).x, _text.GetRenderedValues(true).y);
    }*/

   
}



