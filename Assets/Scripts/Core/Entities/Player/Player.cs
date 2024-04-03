using UnityEngine;
using Zenject;

public class Player : MonoBehaviour
{
    [Inject]
    private SignalBus signalBus;

    [SerializeField]
    private PlayerMovement movement;
    [SerializeField]
    private PlayerStatus status;
    [SerializeField]
    private PlayerInteraction interaction;

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
    }
}
