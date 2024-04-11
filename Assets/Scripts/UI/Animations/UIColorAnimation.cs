using UnityEngine;
using UnityEngine.UI;

public class UIColorAnimation : UIAnimation
{
    [SerializeField]
    private Color inColor;
    [SerializeField]
    private Color outColor;
    [SerializeField]
    private Image target;


    private Color rendererColor;

    protected override void InAnimation()
    {
        target.color.TweenColorTo(inColor, inAnimationDuration, gameObject, ChangeImageColor, OnAnimationFinish);
    }

    protected override void OutAnimation()
    {
        target.color.TweenColorTo(outColor, outAnimationDuration, gameObject, ChangeImageColor, OnAnimationFinish);
    }

    private void ChangeImageColor(Color color)
    {
        target.color = color;
    }

    public override void CancelCurrentAnimation()
    {
        TweenUtils.CancelTween(gameObject, false);
    }
}
