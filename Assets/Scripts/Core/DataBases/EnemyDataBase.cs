using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EnemyDataBase", menuName = "Scriptable Objects/Data Bases/Enemy Data Base")]
public class EnemyDataBase : ScriptableObject
{
    [SerializeField]
    private List<EnemyTypeData> enemyDataBase;

    public Dictionary<EnemyType, Enemy> EnemyData { get; private set; }
    public List<EnemyType> EnemyTypes { get; private set; }

    public void SetUp()
    {
        EnemyData = new Dictionary<EnemyType, Enemy>();
        EnemyTypes = new List<EnemyType>();
        foreach (EnemyTypeData data in enemyDataBase)
        {
            EnemyData.Add(data.Type, data.Enemy);
            EnemyTypes.Add(data.Type);
        }
    }

}

[Serializable]
public struct EnemyTypeData
{
    [SerializeField]
    private EnemyType type;
    [SerializeField]
    private Enemy enemy;

    public Enemy Enemy => enemy;
    public EnemyType Type => type;
}
