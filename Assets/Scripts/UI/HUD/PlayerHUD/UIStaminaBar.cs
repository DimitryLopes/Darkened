using UnityEngine;
using UnityEngine.UI;

public class UIStaminaBar : MonoBehaviour
{
    [SerializeField]
    private Image barImage;

    private Player player;

    public void SetUp(Player player)
    {
        this.player = player;
    }

    public void UpdateBar()
    {
        barImage.fillAmount = player.CurrentStamina / player.MaxStamina;
    }
}
