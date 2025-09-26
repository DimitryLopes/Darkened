using UnityEngine;
using Zenject;

public class InteractionAvailableTriggerCondition : TutorialTriggerCondition
{
    private SignalBus signalBus;
    private bool interactionAvailable = false;

    [Inject]
    public void Construct(SignalBus signalBus)
    {
        this.signalBus = signalBus;
        signalBus.Subscribe<OnPlayerInteractableChangedSignal>(OnInteractableChanged);
    }

    private void OnInteractableChanged(OnPlayerInteractableChangedSignal signal)
    {
        interactionAvailable = signal.Item != null;
    }

    public override bool IsMet()
    {
        return interactionAvailable;
    }

    private void OnDestroy()
    {
        signalBus.Unsubscribe<OnPlayerInteractableChangedSignal>(OnInteractableChanged);
    }
}