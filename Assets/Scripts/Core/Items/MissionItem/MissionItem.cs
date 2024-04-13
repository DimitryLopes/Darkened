public class MissionItem : Item
{
    protected Mission mission;

    public void SetMission(Mission mission)
    {
        this.mission = mission;
    }

    public override void Interact()
    {
        if (CanInteract)
        {
            signalBus.Fire(new OnMissionItemInteractedSignal(this));
            OnInteract();
            DisableInteraction();
        }
    }

    public void EnableInteraction()
    {
        canInteract = true;
    }

    public void DisableInteraction()
    {
        RemoveHighlight();
        canInteract = false;
    }

    protected virtual void OnInteract() { }

}
