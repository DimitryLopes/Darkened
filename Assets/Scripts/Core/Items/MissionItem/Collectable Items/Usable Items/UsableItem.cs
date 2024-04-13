public abstract class UsableItem : CollectableItem
{
    public virtual bool CanUse { get; protected set; }
  
    public virtual void UseItem()
    {
        if (!CanUse) return;

        OnItemUsed();
    }

    protected abstract void OnItemUsed();
    protected abstract void OnItemUseConditionChanged();
}
