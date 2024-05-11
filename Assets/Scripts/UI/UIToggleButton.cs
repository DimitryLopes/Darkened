using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIToggleButton : MonoBehaviour
{
    [SerializeField]
    private Image buttonImage;
    [SerializeField]
    private Image iconImage;
    [SerializeField]
    private Button button;

    [SerializeField]
    public Sprite onIcon;
    [SerializeField]
    public Sprite offIcon;
    [SerializeField]
    public Color onColor;
    [SerializeField]
    public Color offColor;

    public bool IsToggled { get; private set; }

    private void Start()
    {
        button.onClick.AddListener(Toggle);
        UpdateButtonState();
    }

    public void Toggle()
    {
        IsToggled = !IsToggled;
        UpdateButtonState();
    }

    private void UpdateButtonState()
    {
        if (IsToggled)
        {
            iconImage.sprite = onIcon;
            buttonImage.color = onColor;
        }
        else
        {
            iconImage.sprite = offIcon;
            buttonImage.color = offColor;
        }
    }
}
