public class MissionItem : Item
{
    protected override void OnInteract()
    {
        signalBus.Fire(new OnMissionItemInteractedSignal(this));
        DisableInteraction();
    }

    public void EnableInteraction()
    {
        canInteract = true;
        OnInteractionEnabled();
    }

    public void DisableInteraction()
    {
        RemoveHighlight();
        canInteract = false;
        OnInteractionDisabled();
    }

    public virtual void OnInteractionEnabled() { }
    public virtual void OnInteractionDisabled() { }
}
