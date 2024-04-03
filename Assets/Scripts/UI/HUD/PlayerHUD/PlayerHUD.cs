using UnityEngine;
using Zenject;

public class PlayerHUD : Activateable
{
    [Inject]
    private SignalBus signalBus;
    [Inject]
    private EntityManager entityManager;

    [SerializeField]
    private UIStaminaBar staminaBar;

    private void Awake()
    {
        signalBus.Subscribe<OnPlayerStaminaChangedSignal>(OnPlayerStaminaChanged);

        staminaBar.SetUp(entityManager.GetPlayer());
    }

    private void OnPlayerStaminaChanged(OnPlayerStaminaChangedSignal signal)
    {
        staminaBar.UpdateBar();
    }
}
