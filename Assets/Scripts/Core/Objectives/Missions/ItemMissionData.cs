using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Rename Me", menuName = "Scriptable Objects/Mission")]
public class ItemMissionData : MissionData
{
    [SerializeField]
    private MissionItemData itemData;
    public MissionItemData ItemData => itemData;
    public override int ProgressTarget => itemData.Amount;
}

[Serializable]
public struct MissionItemData
{
    [SerializeField]
    private ItemType item;
    [SerializeField, Range(0,100)]
    private int amount;

    public ItemType ItemType => item;
    public int Amount => amount;
}