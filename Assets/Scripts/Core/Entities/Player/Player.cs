using System.Collections.Generic;
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
    private PlayerTorch torch;

    private PlayerStatus status = new PlayerStatus();
    public float CurrentStamina => movement.CurrentStamina;
    public float MaxStamina => status.MaxStamina;
    public PlayerTorch Torch => torch;

    private List<Torch> TorchesInRange = new();
    public bool IsNearTorch => TorchesInRange.Count > 0;

    public void SetUp()
    {
        status.Setup(statusSO);
        movement.SetUp(status, joystick, hud.BottomHUD.SprintButton, signalBus);
        interaction.SetUp(status, signalBus);
        torch.Setup(signalBus);
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
        torch.ActiveTorch();
    }

    public void DeactivateTorch()
    {
        torch.DeactivateTorch();
    }

    public void SetBigTorch()
    {
        torch.SetBigTorch();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag(Constants.LayersAndTags.TORCH_TAG))
        {
            Torch torch = collision.GetComponent<Torch>();
            if (torch != null && !TorchesInRange.Contains(torch))
            {
                TorchesInRange.Add(torch);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(Constants.LayersAndTags.TORCH_TAG))
        {
            Torch torch = collision.GetComponent<Torch>();
            if (torch != null && TorchesInRange.Contains(torch))
            {
                TorchesInRange.Remove(torch);
            }
        }
    }
}
