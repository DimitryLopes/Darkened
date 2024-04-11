using System;
using UnityEngine;

public class UIAnimation : MonoBehaviour
{

    [SerializeField]
    protected float inAnimationDuration = 0.5f;
    [SerializeField]
    protected float outAnimationDuration = 0.5f;
    [SerializeField]
    protected LeanTweenType inEase = LeanTweenType.easeOutExpo;
    [SerializeField]
    protected LeanTweenType outEase = LeanTweenType.easeInExpo;
    [SerializeField]
    private bool loop;

    protected Vector3 originalPosition;
    protected Action onFinishCallback;
    protected LTDescr currentTween;

    private void Start()
    {
        originalPosition = transform.position;
    }

    public void DoInAnimation(Action onFinishCallback = null)
    {
        this.onFinishCallback = onFinishCallback;
        if (loop)
        {
            this.onFinishCallback += () => { DoOutAnimation(); };
        }
        CancelCurrentAnimation();
        InAnimation();
    }


    public void DoOutAnimation(Action onFinishCallback = null)
    {
        this.onFinishCallback = onFinishCallback;
        if (loop)
        {
            this.onFinishCallback += () => { DoInAnimation(); };
        }
        CancelCurrentAnimation();
        OutAnimation();
    }

    public virtual void CancelCurrentAnimation()
    {
        if (currentTween != null)
        {
            LeanTween.cancel(currentTween.uniqueId, !loop);
        }
    }

    protected void OnAnimationFinish()
    {
        Action action = onFinishCallback;
        onFinishCallback = null;
        currentTween = null;
        action?.Invoke();
    }

    protected virtual void InAnimation() { }

    protected virtual void OutAnimation() { }
}