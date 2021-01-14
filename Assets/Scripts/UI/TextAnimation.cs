using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class TextAnimation : MonoBehaviour
{
    private Animator _animator;
    public CanvasGroup canvasGroup;
    [SerializeField]
    AnimationType showAnimation;
    [SerializeField]
    AnimationType hideAnimation;
    
    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void Show()
    {
        canvasGroup.alpha = 1f;
        ShowAnimation();
    }
    
    public void Hide()
    {
        HideAnimation();
    }

    enum AnimationType
    {
        Pop,
        Fade,
    }
    

    public void ShowAnimation()
    {
        if (showAnimation == AnimationType.Pop)
        {
            _animator.SetTrigger("PopIn");
        }

        if (showAnimation == AnimationType.Fade)
        {
            _animator.SetTrigger("FadeIn");
        }
    }

    public void HideAnimation()
    {
        if (hideAnimation == AnimationType.Pop)
        {
            _animator.SetTrigger("PopOut");
        }

        if (hideAnimation == AnimationType.Fade)
        {
            _animator.SetTrigger("FadeOut");
        }
    }

    void Disable()
    {
        canvasGroup.alpha = 0f;
    }
}
