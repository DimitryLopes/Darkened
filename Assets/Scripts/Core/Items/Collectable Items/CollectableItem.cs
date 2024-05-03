public class CollectableItem : Item
{
    protected override void OnInteract()
    {
        Deactivate();
        signalBus.Fire(new OnInventoryItemGetSignal(this, 1));
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
