using UnityEngine;

public class CreepyHand : MonoBehaviour
{
    [SerializeField]
    private RectTransform rectTransform;
    [SerializeField]
    private Transform startPosition;
    [SerializeField]
    private Transform endPosition;
    [SerializeField]
    private UIAnimationComponent wobbleAnimation;

    private void Start()
    {
        PlayWobbleAnimation();
    }

    private void PlayWobbleAnimation()
    {
        wobbleAnimation.PlayInAnimations(PlayWobbleAnimation);
    }

    public void SetDistance(float percentage)
    {
        percentage = Mathf.Clamp01(percentage);

        if (startPosition != null && endPosition != null)
        {
            transform.position = Vector3.Lerp(startPosition.position, endPosition.position, percentage);
        }
    }
}
