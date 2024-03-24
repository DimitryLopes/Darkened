using UnityEngine;

public class UISlideAnimation : UIAnimation
{
    [SerializeField]
    private Vector3 inTarget;
    [SerializeField]
    private Vector3 outTarget;

    protected override void InAnimation()
    {
        currentTween = transform.LeanMoveLocal(inTarget, animationDuration)
            .setEase(inEase)
            .setOnComplete(OnAnimationFinish);
    }

    protected override void OutAnimation()
    {
        currentTween = transform.LeanMoveLocal(outTarget, animationDuration)
            .setEase(outEase)
            .setOnComplete(OnAnimationFinish);
    }
}
