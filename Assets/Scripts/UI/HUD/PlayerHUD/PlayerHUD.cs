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

        staminaBar.SetUp(entityManager.GetPlayer());
    }

    private void OnPlayerStaminaChanged(OnPlayerStaminaChangedSignal signal)
    {
        staminaBar.UpdateBar();
    }

    private void OnPlayerStaminaExausted()
    {
        staminaBar.DoExaustedAnimation();
    }

    private void OnPlayerExaustionRecovered()
    {
        staminaBar.DoStaminaRecoveredAnimation();
    }

    public override void OnActivate()
    {
        staminaBar.UpdateBar();
    }

    public override void OnDeactivate()
    {
        staminaBar.StopAnimations();
    }

}
