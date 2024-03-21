using UnityEngine;
using UnityEngine.Events;

public class UIAnimation : MonoBehaviour
{

    [SerializeField]
    protected float animationDuration = 0.5f;
    [SerializeField]
    protected LeanTweenType inEase = LeanTweenType.easeOutExpo;
    [SerializeField]
    protected LeanTweenType outEase = LeanTweenType.easeInExpo;

    protected Vector3 originalPosition;
    protected UnityAction onFinishCallback;
    protected LTDescr currentTween;

    private void Start()
    {
        originalPosition = transform.position;
    }

    public void DoInAnimation(UnityAction onFinishCallback)
    {
        this.onFinishCallback = onFinishCallback;
        CancelCurrentAnimation();
        InAnimation();
    }


    public void DoOutAnimation(UnityAction onFinishCallback)
    {
        this.onFinishCallback = onFinishCallback;
        CancelCurrentAnimation();
        OutAnimation();
    }

    private void CancelCurrentAnimation()
    {
        if (currentTween != null)
        {
            LeanTween.cancel(currentTween.uniqueId, true); // Complete the previous animation
        }
    }

    protected void OnAnimationFinish()
    {
        onFinishCallback?.Invoke();
        onFinishCallback = null;
        currentTween = null;
    }

    protected virtual void InAnimation() { }

    protected virtual void OutAnimation() { }
}