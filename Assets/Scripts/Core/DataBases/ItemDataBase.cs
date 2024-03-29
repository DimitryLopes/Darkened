using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "ItemDataBase", menuName = "Scriptable Objects/Data Bases/Item Data Base")]
public class ItemDataBase : ScriptableObject
{
    [SerializeField]
    private List<ItemTypeData> itemDataBase;

    private Dictionary<ItemType, Item> itemDatas;

    public Dictionary<ItemType, Item> ItemDatas => itemDatas;

    public void SetUp()
    {
        itemDatas = new Dictionary<ItemType, Item>();
        foreach(ItemTypeData data in itemDataBase)
        {
            itemDatas.Add(data.Type, data.Item);
        }
    }
}

[Serializable]
public struct ItemTypeData
{
    [SerializeField]
    private ItemType type;
    [SerializeField]
    private Item item;

    public Item Item => item;
    public ItemType Type => type;
}
