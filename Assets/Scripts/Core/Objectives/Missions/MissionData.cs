using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Rename Me", menuName = "Scriptable Objects/Mission")]
public class MissionData : ScriptableObject
{
    [SerializeField]
    private string description;
    [SerializeField]
    private MissionItemData itemData;

    public string Description => description;
    public MissionItemData ItemData => itemData;
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