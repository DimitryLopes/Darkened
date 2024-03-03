using System.Collections.Generic;
using UnityEngine;
using System;

public class LevelDataBase : MonoBehaviour
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
