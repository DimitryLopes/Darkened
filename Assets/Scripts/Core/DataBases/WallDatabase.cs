using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "WallDataBase", menuName = "Scriptable Objects/Data Bases/Wall Data Base")]
public class WallDatabase : ScriptableObject
{
    [SerializeField]
    private List<WallTypeData> wallDatabase;

    private Dictionary<WallType, MazeWall> wallDatas;

    public Dictionary<WallType, MazeWall> Walls => wallDatas;

    public void SetUp()
    {
        wallDatas = new Dictionary<WallType, MazeWall>();
        foreach (WallTypeData data in wallDatabase)
        {
            wallDatas.Add(data.Type, data.Wall);
        }
    }
}

[Serializable]
public struct WallTypeData
{
    [SerializeField]
    private WallType type;
    [SerializeField]
    private MazeWall wall;

    public MazeWall Wall => wall;
    public WallType Type => type;
}
