using Zenject;
using System;
using UnityEngine;
using System.Collections.Generic;

public class ItemFactory : PlaceholderFactory<Type, Item>
{
    private readonly DiContainer _container;
    private Dictionary<ItemType, Item> itemDictionary;

    public ItemFactory(DiContainer container, List<ItemTypeData> ItemTypeList)
    {
        _container = container;
        itemDictionary = new Dictionary<ItemType, Item>();

        for (int i = 0; i < ItemTypeList.Count; i++)
        {
            itemDictionary.Add(ItemTypeList[i].Type, ItemTypeList[i].Item);
        }
    }

    public override Item Create(Type itemType)
    {
        if (!typeof(Item).IsAssignableFrom(itemType))
        {
            throw new ArgumentException($"Type {itemType.Name} does not implement IItem interface");
        }

        return (Item)_container.Instantiate(itemType);
    }

    /// <summary>
    /// This is the method you call to get a new instance of an IItem
    /// </summary>
    /// <param name="itemType">The item type</param>
    /// <returns></returns>
    public Item CreateItem(ItemType itemType)
    {
        Type type = itemDictionary[itemType].GetType();
        return Create(type);
    }

}

[Serializable]
public struct ItemTypeData
{
    public ItemType Type;
    public Item Item;
}