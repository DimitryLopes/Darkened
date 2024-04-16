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
    }

    public void DisableInteraction()
    {
        RemoveHighlight();
        canInteract = false;
    }

}
