public class OnEnemySpawnedSignal
{
    public Enemy Enemy { get; }

    public OnEnemySpawnedSignal(Enemy enemy)
    {
        Enemy = enemy;
    }
}