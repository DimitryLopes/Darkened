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

    public void Show(string text, Camera camera)
    {
        Activate();
        canvas.worldCamera = camera;
        canvas.sortingLayerName = Constants.LayersAndTags.FLOATING_TEXT_SORTING_LAYER;
        this.text.text = text;
        animation.PlayInAnimations(Hide);
    }

    private void Hide()
    {
        animation.PlayOutAnimations(Deactivate);
    }
}
