public class CollectableItem : Item
{
    protected override void OnInteract()
    {
        Deactivate();
        signalBus.Fire(new OnInventoryItemGetSignal(this, 1));
    }
}
