using UnityEngine;
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
    private PlayerMovement movement;
    [SerializeField]
    private PlayerStatusSO statusSO;
    [SerializeField]
    private PlayerInteraction interaction;
    [SerializeField]
    private PlayerLightDetector lightDetector;
    [SerializeField]
    private PlayerTorch torch;

    private PlayerStatus status = new PlayerStatus();
    public float CurrentStamina => movement.CurrentStamina;
    public float MaxStamina => status.MaxStamina;
    public PlayerTorch Torch => torch;

    public bool IsLit => lightDetector.IsLit;

    public void SetUp()
    {
        status.Setup(statusSO);
        movement.SetUp(status, joystick, hud.BottomHUD.SprintButton, signalBus);
        interaction.SetUp(status, signalBus);
        lightDetector.Setup(signalBus);
        torch.Setup(signalBus);
    }

    public void ResetPlayer(DifficultyData data)
    {
        movement.ResetMovement();
        ToggleActing(true);
        SetDefaultTorch();
        ActivateTorch();
        torch.SetBurnSpeedModifier(data);
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

    public void SetDefaultTorch()
    {
        torch.SetDefaultTorch();
    }

    public void SetSpectralTorch()
    {
        torch.SetSpectralTorch();
    }

    public void ActivateTorch()
    {
        torch.ActivateTorch();
    }

    public void DeactivateTorch()
    {
        torch.DeactivateTorch();
    }

    public void SetBigTorch()
    {
        torch.SetBigTorch();
    }
}
