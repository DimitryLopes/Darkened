public abstract class TrapItem : Item, ITrap
{
    public override void OnActivate()
    {
        base.OnActivate();
        canInteract = false;
    }

    public abstract void Trigger();
}
