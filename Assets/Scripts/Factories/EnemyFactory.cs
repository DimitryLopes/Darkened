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

    public Enemy Create(EnemyType type)
    {
        Enemy prefab = dataBase.EnemyData[type];
        Enemy instance = container.InstantiatePrefabForComponent<Enemy>(prefab);

        return instance;
    }
}
