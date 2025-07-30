using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelDataBase", menuName = "Scriptable Objects/Data Bases/Level Data Base")]
public class LevelDataBase : ScriptableObject
{
    [SerializeField]
    private List<LevelData> levelDatas;
    [SerializeField]
    private List<Torch> torches;

    public List<LevelData> LevelDatas => levelDatas;
    public List<Torch> Torches => torches;

    public int GetLevelID(LevelData data)
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

    public void SetUp()
    {
        for (int i = 0; i < levelDatas.Count; i++)
        {
            levelDatas[i].ID = i + 1;
            levelDatas[i].SetPersistenceKey();
        }
    }
}

