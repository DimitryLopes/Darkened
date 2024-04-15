public class CollectableMissionItem : MissionItem
{
    protected override void OnInteract()
    {
        base.OnInteract();
        Deactivate();
        signalBus.Fire(new OnInventoryItemGetSignal(this, 1));
    }
}
