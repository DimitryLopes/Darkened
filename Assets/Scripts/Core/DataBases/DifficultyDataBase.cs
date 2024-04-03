using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DifficultyDataBase", menuName = "Scriptable Objects/Data Bases/Difficulty Data Base")]
public class DifficultyDataBase : ScriptableObject
{
    [SerializeField]
    private List<DifficultyDataInfo> difficulties;

    public readonly Dictionary<DifficultyType, DifficultyData> DifficultyDictionary = new Dictionary<DifficultyType, DifficultyData>();
    public readonly List<DifficultyData> DifficultyList = new List<DifficultyData>();

    public void SetUp()
    {
        foreach (DifficultyDataInfo difficulty in difficulties)
        {
            DifficultyDictionary.Add(difficulty.Type, difficulty.Data);
            DifficultyList.Add(difficulty.Data);
        }
    }

    public DifficultyData GetRandomData()
    {
        return DifficultyDictionary.GetRandom().Value;
    }
}

[Serializable]
public struct DifficultyDataInfo
{
    [SerializeField]
    private DifficultyType difficultyType;
    [SerializeField]
    private DifficultyData difficultyData;

    public DifficultyType Type => difficultyType;
    public DifficultyData Data => difficultyData;
}

public enum DifficultyType
{
    Easy,
    Normal,
    Hard,
    Darkened,
}
