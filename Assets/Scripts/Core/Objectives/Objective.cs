using System.Collections.Generic;
using UnityEngine;

public class Objective : ScriptableObject, IObjective
{
    [SerializeField]
    private string description;

    [SerializeField]
    private List<ItemType> requiredItems;

    public bool IsCompleted { get; private set; }

    public string Description => description;

    public float Progress { get; private set; }

    public void CompleteMission()
    {
        IsCompleted = true;
    }

    public void CompleteObjective()
    {

    }

    public void Clear()
    {
        Progress = 0;
    }

    public List<ItemType> GetRequiredItems()
    {
        return requiredItems;
    }

    public void StartObjective()
    {
        IsCompleted = false;
    }

    public void UpdateProgress(float amount)
    {
        Progress += amount;
    }
}
