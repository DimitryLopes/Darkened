using TMPro;
using UnityEngine;

public class FloatingText : Activateable
{
    [SerializeField]
    private new UIAnimationComponent animation;
    [SerializeField]
    private TextMeshProUGUI text;

    public void Show(string text)
    {
        Activate();
        this.text.text = text;
        animation.PlayOutAnimations(Hide);
    }

    private void Hide()
    {
        animation.PlayOutAnimations(Deactivate);
    }
}
