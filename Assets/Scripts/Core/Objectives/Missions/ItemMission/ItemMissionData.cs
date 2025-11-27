using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Rename Me", menuName = "Scriptable Objects/Datas/Mission Datas/Item Mission Data")]
public class ItemMissionData : MissionData
{
    [SerializeField]
    private ItemType itemType;

    [SerializeField]
    private List<MissionItemData> dataList;

    public ItemType Item => itemType;
    public Dictionary<DifficultyType, MissionItemData> ItemDatas { get; private set;} = new();

    public void SetUp()
    {
        foreach(MissionItemData data in dataList)
        {
            ItemDatas.Add(data.Difficulty, data);
        }
    }

    public void Clear()
    {
        ItemDatas.Clear();
    }

    public override int GetTargetProgress(DifficultyType difficulty)
    {
        return ItemDatas[difficulty].Amount;
    }
}

[Serializable]
public struct MissionItemData
{
    [SerializeField]
    private DifficultyType difficultyType;
    [SerializeField]
    private int amount;

    public int Amount => amount;
    public DifficultyType Difficulty => difficultyType;
}