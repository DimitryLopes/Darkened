using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelDataBase", menuName = "Scriptable Objects/Data Bases/Level Data Base")]
public class LevelDataBase : ScriptableObject
{
    [SerializeField]
    private List<RandomLevelData> levelDatas;
    [SerializeField]
    private List<MazeTorch> torches;

    public List<RandomLevelData> LevelDatas => levelDatas;
    public List<MazeTorch> Torches => torches;

    public int GetLevelID(RandomLevelData data)
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
            levelDatas[i].ID = i;
            levelDatas[i].SetPersistenceKey();
        }
    }
}

