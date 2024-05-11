using UnityEngine;
using UnityEngine.UI;

public class UIInteractionButton : MonoBehaviour
{
    [SerializeField]
    private Sprite defaultIcon;
    [SerializeField]
    private Button button;
    [SerializeField]
    private Image icon;

    public Button Button => button;

    public void SetIcon(Sprite sprite)
    {
        icon.sprite = sprite == null ? defaultIcon : sprite;
    }
}
