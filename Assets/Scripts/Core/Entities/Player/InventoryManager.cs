using System;
using System.Collections.Generic;
using Zenject;

public class InventoryManager
{
    private const int INVENTORY_SIZE = 4;

    private HUDManager hudManager;
    private SignalBus signalBus;

    private Dictionary<ItemType, InventoryItemData> inventoryItems = new();

    public InventoryManager(SignalBus signalBus, HUDManager hudManager)
    {
        this.hudManager = hudManager;
        this.signalBus = signalBus;

        signalBus.Subscribe<OnInventoryItemGetSignal>(OnItemGet);
        signalBus.Subscribe<OnPlayerUsedItemSignal>(UseItem);

        hudManager.CreateItemViews(INVENTORY_SIZE);
    }

    private void OnItemGet(OnInventoryItemGetSignal signal)
    {
        ItemType type = signal.Item.Type;
        InventoryItemData data;
        if (!inventoryItems.ContainsKey(type))
        {
            data = new InventoryItemData(signal.Item, signal.Amount);
            inventoryItems.Add(type, data);
            hudManager.CreateItemView(data);
        }
        else
        {
            IncreaseItemAmount(type, signal.Amount);
        }
        hudManager.UpdateItemView(inventoryItems[type]);
    }

    public void UseItem(OnPlayerUsedItemSignal signal)
    {
        signal.Item.Use();
    }

    public void DecreaseItemAmount(ItemType type, int amount = 1)
    {
        if (!HasEnoughItem(type, amount)) return;

        InventoryItemData data = inventoryItems[type];
        data.Amount -= amount;
        inventoryItems[type] = data;
        hudManager.UpdateItemView(inventoryItems[type]);
    }

    public T GetItem<T>(ItemType type) where T : Item
    {
        if (!HasEnoughItem(type)) return null;

        return inventoryItems[type].Item as T;
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

    public void Clear()
    {
        inventoryItems.Clear();
    }
}

public struct InventoryItemData
{
    public InventoryItemData(Item item, int amount)
    {
        Item = item;
        Amount = amount;
    }

    public Item Item { get; private set; }
    public int Amount { get; set; }
}