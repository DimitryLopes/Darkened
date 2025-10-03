using TMPro;
using UnityEngine;

public class FloatingText : Activateable
{
    [SerializeField]
    private new UIAnimationComponent animation;
    [SerializeField]
    private TextMeshProUGUI text;
    [SerializeField]
    private Canvas canvas;

    private float duration;

    public void Show(string text, Camera camera, float duration)
    {
        Activate();
        this.duration = duration;
        canvas.worldCamera = camera;
        canvas.sortingLayerName = Constants.LayersAndTags.FLOATING_TEXT_SORTING_LAYER;
        this.text.text = text;
        if (duration < 0)
        {
            animation.PlayInAnimations();

        }
        else
        {
            animation.PlayInAnimations(Hide);
        }
    }

    private void Hide()
    {
        animation.SetOutDelay(duration);
        animation.PlayOutAnimations(Deactivate);
    }

    public void InstantHide()
    {
        animation.SetOutDelay(0f);
        animation.PlayOutAnimations(Deactivate);
    }
}
