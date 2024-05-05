using UnityEngine;
using Zenject;

public class EnemyFactory 
{
    private readonly DiContainer container;
    private readonly EnemyDataBase dataBase;

    public EnemyFactory(EnemyDataBase dataBase, DiContainer container)
    {
        this.container = container;
        this.dataBase = dataBase;
    }

    public Enemy Create(EnemyType type, Transform parent)
    {
        Enemy prefab = dataBase.EnemyData[type];
        Enemy instance = container.InstantiatePrefabForComponent<Enemy>(prefab, parent);

        return instance;
    }
}
