public class WanderingEnemyStateData : EnemyStateData
{
    public Maze Maze { get; private set; }
    public EnemyBase Enemy { get; private set; }

    public WanderingEnemyStateData(Maze maze, EnemyBase enemy)
    {
        Maze = maze;
        Enemy = enemy;
    }
}
