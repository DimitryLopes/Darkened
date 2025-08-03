using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "ItemDataBase", menuName = "Scriptable Objects/Data Bases/Item Data Base")]
public class WallDatabase : ScriptableObject
{
    [SerializeField]
    private List<WallTypeData> itemDataBase;

    private Dictionary<WallType, MazeWall> itemDatas;

    public Dictionary<WallType, MazeWall> Walls => itemDatas;

    public void SetUp()
    {
        itemDatas = new Dictionary<WallType, MazeWall>();
        foreach (WallTypeData data in itemDataBase)
        {
            itemDatas.Add(data.Type, data.Wall);
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
