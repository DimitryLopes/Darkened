public abstract class UsableMissionItem : CollectableMissionItem, IUsable
{    public virtual bool CanUse { get; protected set; }

    public virtual void Use()
    {
        if (!CanUse) return;
        Deactivate();
        OnItemUsed();
    }

    public override void OnActivate()
    {
        base.OnActivate();
        CanUse = true;
    }

    public abstract void OnItemUsed();
}
