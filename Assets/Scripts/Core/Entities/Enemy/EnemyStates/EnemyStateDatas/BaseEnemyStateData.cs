public class BaseEnemyStateData
{
    public Maze Maze { get; private set; }
    public EnemyBase Enemy { get; private set; }

    public BaseEnemyStateData(Maze maze, EnemyBase enemy)
    {
        Maze = maze;
        Enemy = enemy;
    }
}
