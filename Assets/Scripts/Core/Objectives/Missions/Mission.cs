using System.Collections.Generic;
using UnityEngine;
using System;
using Zenject;

[CreateAssetMenu(fileName = "Rename Me", menuName = "Scriptable Objects/Mission")]
public class Mission : ScriptableObject
{
    [SerializeField]
    private MissionItemData itemData;

    private SignalBus signalBus;
    public MissionItemData ItemData => itemData;

    private int progress;

    public List<ItemType> GetRequiredItems()
    {
        List<ItemType> items = new List<ItemType>();

        for (int i = 0; i < itemData.Amount; i++)
        {
            items.Add(itemData.ItemType);
        }

        return items;
    }

    public void OnMissionProgress(ItemType type)
    {
        if (itemData.ItemType == type)
        {
            progress += 1;
            if (progress == itemData.Amount)
            {
                CompleteMission();
            }
        }
    }

    private void CompleteMission()
    {
        signalBus.Fire(new OnMissionCompletedSignal(this));
    }

    public void SetUp(MissionItemData datas, SignalBus signalBus)
    {
        progress = 0;
        itemData = datas;
        this.signalBus = signalBus;
    }
}

[Serializable]
public struct MissionItemData
{
    [SerializeField]
    private ItemType item;
    [SerializeField]
    private int amount;

    public ItemType ItemType => item;
    public int Amount => amount;
}