using UnityEngine.Events;

public class WaitingEnemyStateData : BaseEnemyStateData
{
    public float WaitingTime { get; private set; }
    public WaitingEnemyStateData(Enemy enemy, bool isSprinting, float waitingTime,
        UnityAction onDeactivateCallback, UnityAction onActivateCallback, UnityAction onStateCompletedCallback) : base(enemy, isSprinting, onDeactivateCallback, onActivateCallback, onStateCompletedCallback)
    {
        WaitingTime = waitingTime;
    }
}
