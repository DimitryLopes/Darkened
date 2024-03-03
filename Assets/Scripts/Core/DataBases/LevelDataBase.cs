using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "LevelDataBase", menuName = "Scriptable Objects/Data Bases/Level Data Base")]
public class LevelDataBase : ScriptableObject
{
    [SerializeField]
    private List<LevelData> levelDatas;

    public List<LevelData> LevelDatas => levelDatas;
}

[Serializable]
public struct LevelData
{
    [SerializeField]
    private int levelID;

    [SerializeField]
    private MazeData data;

    public MazeData Data => data;
    public int LevelID => levelID;
}
