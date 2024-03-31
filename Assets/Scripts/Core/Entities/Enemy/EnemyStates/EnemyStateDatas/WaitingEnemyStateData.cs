using UnityEngine.Events;

public class WaitingEnemyStateData : BaseEnemyStateData
{
    public float WaitingTime { get; private set; }
    public WaitingEnemyStateData(Maze maze, Enemy enemy, bool isSprinting, float waitingTime,
        UnityAction onDeactivateCallback, UnityAction onActivateCallback, UnityAction onStateCompletedCallback) : base(maze, enemy, isSprinting, onDeactivateCallback, onActivateCallback, onStateCompletedCallback)
    {
        WaitingTime = waitingTime;
    }
}
