using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "LevelDataBase", menuName = "Scriptable Objects/Data Bases/Level Data Base")]
public class LevelDataBase : ScriptableObject
{
    [SerializeField]
    private List<LevelData> levelDatas;
    [SerializeField]
    private List<MazeTorch> torches;

    public List<LevelData> LevelDatas => levelDatas;
    public List<MazeTorch> Torches => torches;
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
