public class MissionItem : Item
{
    protected Mission mission;

    public void SetMission(Mission mission)
    {
        this.mission = mission;
    }

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
