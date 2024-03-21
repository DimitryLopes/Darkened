public class UISlideAnimation : UIAnimation
{
    protected override void InAnimation()
    {

        currentTween = LeanTween.moveX(gameObject, originalPosition.x, animationDuration)
            .setEase(inEase)
            .setOnComplete(OnAnimationFinish); // Clear currentTween reference when animation completes
    }

    protected override void OutAnimation()
    {
        float targetX = originalPosition.x + 100f; // Adjust this value based on your desired slide distance
        currentTween = LeanTween.moveX(gameObject, targetX, animationDuration)
            .setEase(outEase)
            .setOnComplete(OnAnimationFinish); // Clear currentTween reference when animation completes
    }
}
