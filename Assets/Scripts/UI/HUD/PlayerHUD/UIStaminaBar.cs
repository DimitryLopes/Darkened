using UnityEngine;
using UnityEngine.UI;

public class UIStaminaBar : MonoBehaviour
{
    [SerializeField]
    private Image barImage;

    [SerializeField, Header("Animation")]
    private UIColorAnimation exaustedAnimation;
    [SerializeField]
    private UIColorAnimation recoveredAnimation;

    private Player player;

    public void SetUp(Player player)
    {
        this.player = player;
    }

    public void UpdateBar()
    {
        barImage.fillAmount = player.CurrentStamina / player.MaxStamina;
    }

    public void DoExaustedAnimation()
    {
        exaustedAnimation.DoInAnimation();
    }

    public void DoStaminaRecoveredAnimation()
    {
        exaustedAnimation.CancelCurrentAnimation();
        recoveredAnimation.DoInAnimation(DoRecoveryOutAnimation);
    }

    private void DoRecoveryOutAnimation()
    {
        recoveredAnimation.DoOutAnimation();
    }

    public void StopAnimations()
    {
        exaustedAnimation.CancelCurrentAnimation();
        recoveredAnimation.CancelCurrentAnimation();
    }
}
