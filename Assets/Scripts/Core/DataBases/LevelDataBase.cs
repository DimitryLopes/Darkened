using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "LevelDataBase", menuName = "Scriptable Objects/Data Bases/Level Data Base")]
public class LevelDataBase : ScriptableObject
{
    [SerializeField]
    private List<MazeData> levelDatas;
    [SerializeField]
    private List<MazeTorch> torches;

    public List<MazeData> LevelDatas => levelDatas;
    public List<MazeTorch> Torches => torches;

    public int GetLevelID(MazeData data)
    {
        for(int i = 0; i < levelDatas.Count; i++)
        {
            if(data == levelDatas[i])
            {
                return i;
            }
        }
        Debug.LogError("There was no level data with specified data in the data base");
        return -1;
    }
}

