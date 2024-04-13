using System;
using System.Collections.Generic;
using Zenject;

public class InventoryManager
{
    private HUDManager hudManager;
    private SignalBus signalBus;

    public Item SelectedItem { get; private set; }

    private Dictionary<ItemType,InventoryItemData> inventoryItems = new();

    public InventoryManager(SignalBus signalBus, HUDManager hudManager)
    {
        this.hudManager = hudManager;
        this.signalBus = signalBus;

        signalBus.Subscribe<OnInventoryItemGetSignal>(OnItemGet);
    }

    private void OnItemGet(OnInventoryItemGetSignal signal)
    {
        ItemType type = signal.Item.Type;
        InventoryItemData data;
        if (!inventoryItems.ContainsKey(type))
        {
            data = new InventoryItemData();
            inventoryItems.Add(type, data);
            hudManager.CreateItemView(data, SelectItem);
        }
        data = inventoryItems[type];
        IncreaseItemAmount(type, signal.Amount);
        hudManager.UpdateItemView(data);
    }

    public void UseItem(UsableItem item)
    {
        if (!inventoryItems.ContainsKey(item.Type)) return;

        item.UseItem();
    }

    public void ReduceItemAmount(ItemType type, int amount = 1)
    {
        if (!HasEnoughItem(type, amount)) return;

        InventoryItemData data = inventoryItems[type];
        data.Amount -= amount;
        inventoryItems[type] = data;
    }

    public void IncreaseItemAmount(ItemType type, int amount = 1)
    {
        InventoryItemData data = inventoryItems[type];
        data.Amount += amount;
        inventoryItems[type] = data;
    }

    public bool HasEnoughItem(ItemType type, int amount = 1)
    {
        if (!inventoryItems.ContainsKey(type)) return false;

        else return inventoryItems[type].Amount >= amount;
    }

    private void SelectItem(Item item)
    {
        SelectedItem = item;
        signalBus.Fire(new OnInventoryItemSelectedSignal(item));
    }

    public void Clear()
    {
        InventoryItemData data;
       foreach (ItemType type in Enum.GetValues(typeof(ItemType)))
        {
            if (!inventoryItems.ContainsKey(type)) continue;

            data = inventoryItems[type];
            data.Amount = 0;
            inventoryItems[type] = data;
        }
    }
}

public struct InventoryItemData
{
    public Item Item { get; private set; }
    public int Amount { get; set; }
}