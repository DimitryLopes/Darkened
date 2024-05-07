using UnityEngine;
using UnityEngine.Rendering.Universal;
using Zenject;

public class Player : MonoBehaviour
{
    [Inject]
    private SignalBus signalBus;

    [SerializeField]
    private Light2D playerTorch;
    [SerializeField]
    private PlayerMovement movement;
    [SerializeField]
    private PlayerStatus status;
    [SerializeField]
    private PlayerInteraction interaction;
    [SerializeField]
    private Color defaultTorchColor;
    [SerializeField]
    private Color spectalTorchColor;

    public float CurrentStamina => movement.CurrentStamina;
    public float MaxStamina => status.MaxStamina;

    public void SetUp()
    {
        movement.SetUp(status, signalBus);
        interaction.SetUp(status);
    }

    public void ToggleActing(bool value)
    {
        movement.ToggleActing(value);
        interaction.ToggleActing(value);
        if (!value) return;

        movement.ResetMovement();
    }

    #region Torch
    public void ActiveTorch()
    {
        playerTorch.enabled = true;
    }

    public void DeactivateTorch()
    {
        playerTorch.enabled = false;
    }

    public void SetBigTorch()
    {
        playerTorch.pointLightOuterRadius = Constants.Player.PLAYER_BIG_TORCH_SIZE;
    }

    public void SetSmallTorch()
    {
         playerTorch.pointLightOuterRadius = Constants.Player.PLAYER_SMALL_TORCH_SIZE;
    }

    public void SetDefaultTorch()
    {
        playerTorch.pointLightOuterRadius = Constants.Player.PLAYER_DEFAULT_TORCH_SIZE;
        playerTorch.shadowsEnabled = true;
        playerTorch.color = defaultTorchColor;
    }

    public void SetSpectralTorch()
    {
        playerTorch.shadowsEnabled = false;
        playerTorch.color = spectalTorchColor;
    }
    #endregion
}
