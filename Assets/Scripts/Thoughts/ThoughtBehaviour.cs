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
}



