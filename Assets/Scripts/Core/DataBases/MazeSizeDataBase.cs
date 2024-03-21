using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MazeSizeDataBase", menuName = "Scriptable Objects/Data Bases/Maze Size Data Base")]
public class MazeSizeDataBase : ScriptableObject
{
    [SerializeField]
    private List<MazeSizeData> datas;

    public List<MazeSizeData> GetSizeDatas()
    {
        return datas;
    }
}
