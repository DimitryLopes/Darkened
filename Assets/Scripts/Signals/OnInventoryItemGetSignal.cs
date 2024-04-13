public class OnInventoryItemGetSignal
{
    public OnInventoryItemGetSignal(Item item, int amount)
    {
        Item = item;
        Amount = amount;
    }

    public Item Item { get; private set; }
    public int Amount { get; private set; }
}
