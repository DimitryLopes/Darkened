public class OnInventoryItemSelectedSignal 
{
    public OnInventoryItemSelectedSignal(Item item)
    {
        Item = item;
    }

    public Item Item { get; private set; }
}
