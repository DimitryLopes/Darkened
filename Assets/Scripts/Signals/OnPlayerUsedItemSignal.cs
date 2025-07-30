public class OnPlayerUsedItemSignal
{
    public UsableItem Item { get; private set; }

    public OnPlayerUsedItemSignal(UsableItem item)
    {
        Item = item;
    }
}
