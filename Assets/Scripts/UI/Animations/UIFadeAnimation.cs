using UnityEngine;

public class UIFadeAnimation : UIAnimation
{
    [SerializeField]
    private CanvasGroup target;
    [SerializeField]
    private float inTarget = 1;
    [SerializeField]
    private float outTarget = 0;

    protected override void InAnimation()
    {
        target.alpha = 0;
        currentTween = target.LeanAlpha(inTarget, animationDuration)
            .setEase(inEase)
            .setOnComplete(OnAnimationFinish);
    }

    protected override void OutAnimation()
    {
        target.alpha = 1;
        currentTween = target.LeanAlpha(outTarget, animationDuration)
            .setEase(outEase)
            .setOnComplete(OnAnimationFinish);
    }
}
