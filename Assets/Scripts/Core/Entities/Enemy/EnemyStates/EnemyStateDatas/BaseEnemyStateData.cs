public class BaseEnemyStateData
{
    public Maze Maze { get; private set; }
    public Enemy Enemy { get; private set; }

    public BaseEnemyStateData(Maze maze, Enemy enemy)
    {
        Maze = maze;
        Enemy = enemy;
    }
}
