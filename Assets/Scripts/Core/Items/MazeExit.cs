using Zenject;
using UnityEngine;

public class MazeExit : Item, IInteractable, IObjective
{
    private SignalBus signalBus;

    [Inject]
    public void Create(SignalBus signalBus)
    {
        this.signalBus = signalBus;
    }

    public bool CanInteract { get; private set; }

    public void Interact()
    {
        CompleteObjective();
    }

    public void CompleteObjective()
    {
        throw new System.NotImplementedException();
    }
}


