using UnityEngine;
using UnityEngine.Rendering.Universal;
using Zenject;

public class Player : MonoBehaviour
{
    [Inject]
    private SignalBus signalBus;
    [Inject]
    private Joystick joystick;
    [Inject]
    private HUD hud;

    [SerializeField]
    private Light2D playerTorch;
    [SerializeField]
    private PlayerMovement movement;
    [SerializeField]
    private PlayerStatusSO statusSO;
    [SerializeField]
    private PlayerInteraction interaction;
    [SerializeField]
    private Color defaultTorchColor;
    [SerializeField]
    private Color spectalTorchColor;

    private PlayerStatus status;
    public float CurrentStamina => movement.CurrentStamina;
    public float MaxStamina => status.MaxStamina;

    public void SetUp()
    {
        status.Setup(statusSO);
        movement.SetUp(status, joystick, hud.BottomHUD.SprintButton, signalBus);
        interaction.SetUp(status, signalBus);
    }

    public void ResetPlayer()
    {
        movement.ResetMovement();
        ToggleActing(true);
        SetDefaultTorch();
        status.ResetStatus();
    }

    public void ToggleActing(bool value)
    {
        movement.ToggleActing(value);
        interaction.ToggleActing(value);
    }

    public void ApplyStatusEffect(StatusEffectData data)
    {
        status.ApplyStatusEffect(data.Multiplier, data.Stat, data.Duration);
    }

    private void Update()
    {
        status.UpdateStatusEffects(Time.deltaTime);
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
