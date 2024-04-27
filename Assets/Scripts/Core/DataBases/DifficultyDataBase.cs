using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DifficultyDataBase", menuName = "Scriptable Objects/Data Bases/Difficulty Data Base")]
public class DifficultyDataBase : ScriptableObject
{
    [SerializeField]
    private List<DifficultyData> difficulties;

    public List<DifficultyData> DifficultyList => difficulties;

    public DifficultyData GetRandomData()
    {
        return difficulties.GetRandom();
    }
}

public enum DifficultyType
{
    Easy,
    Normal,
    Hard,
    Darkened,
}
