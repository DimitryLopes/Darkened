using UnityEngine;
using UnityEngine.UI;

public class UIStaminaBar : MonoBehaviour
{
    [SerializeField]
    private Image barImage;

    [SerializeField, Header("Animation")]
    private UIAnimationComponent exaustedAnimation;
    [SerializeField]
    private UIAnimationComponent recoveredAnimation;

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
        exaustedAnimation.PlayInAnimations();
    }

    public void DoStaminaRecoveredAnimation()
    {
        exaustedAnimation.StopCurrentAnimations();
        recoveredAnimation.PlayInAnimations(DoRecoveryOutAnimation);
    }

    private void DoRecoveryOutAnimation()
    {
        recoveredAnimation.PlayOutAnimations();
    }

    public void StopAnimations()
    {
        exaustedAnimation.StopCurrentAnimations();
        recoveredAnimation.StopCurrentAnimations();
    }
}
