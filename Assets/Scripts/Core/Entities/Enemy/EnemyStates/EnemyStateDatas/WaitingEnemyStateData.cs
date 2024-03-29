public class WaitingEnemyStateData : BaseEnemyStateData
{
    public float WaitingTime { get; private set; }

    public WaitingEnemyStateData(Maze maze, Enemy enemy, float waitingTime) : base(maze, enemy)
    {
        WaitingTime = waitingTime;
    }
}
