public struct ItemPositionData
{
    public Item Item;
    public PresetItemData PresetData;
    public ItemPositionData(Item item, PresetItemData presetData)
    {
        Item = item;
        PresetData = presetData;
    }
}