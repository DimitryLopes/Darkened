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
        signalBus.Subscribe<OnPlayerStaminaExaustedSignal>(OnPlayerStaminaExausted);
        signalBus.Subscribe<OnPlayerExaustedRecoveredSignal>(OnPlayerExaustionRecovered);
    }

    private void OnPlayerStaminaChanged(OnPlayerStaminaChangedSignal signal)
    {
        staminaBar.UpdateBar();
    }

    private void OnPlayerStaminaExausted()
    {
        staminaBar.PlayExaustedAnimation();
    }

    private void OnPlayerExaustionRecovered()
    {
        staminaBar.PlayStaminaRecoveredAnimation();
    }

    public override void OnActivate()
    {
        base.OnActivate();
        staminaBar.SetUp(entityManager.GetPlayer());
        staminaBar.UpdateBar();
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();
        staminaBar.StopAnimations();
    }

}
