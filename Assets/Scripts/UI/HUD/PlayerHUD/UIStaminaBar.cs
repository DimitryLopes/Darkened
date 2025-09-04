using UnityEngine;
using UnityEngine.UI;

public class UIStaminaBar : MonoBehaviour
{
    [SerializeField]
    private Image barImage;

    [SerializeField, Header("Animation")]
    private UIAnimationComponent staminaAnimation;

    private Player player;

    public void SetUp(Player player)
    {
        this.player = player;
        UpdateBar();
    }

    public void UpdateBar()
    {
        barImage.fillAmount = player.CurrentStamina / player.MaxStamina;
    }

    public void PlayExaustedAnimation()
    {
        staminaAnimation.PlayInAnimations(PlayExaustedAnimation);
    }

    public void PlayStaminaRecoveredAnimation()
    {
        staminaAnimation.StopCurrentAnimations();
        staminaAnimation.PlayOutAnimations();
    }

    public void StopAnimations()
    {
        staminaAnimation.StopCurrentAnimations();
    }
}
