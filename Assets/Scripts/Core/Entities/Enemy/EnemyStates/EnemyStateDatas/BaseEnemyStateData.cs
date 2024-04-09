using UnityEngine.Events;

public class BaseEnemyStateData
{
    public BaseEnemyStateData(Enemy enemy, bool isSprinting,
        UnityAction onDeactivateCallback, UnityAction onActivateCallback, UnityAction onStateCompletedCallback)
    {
        Enemy = enemy;
        IsSprinting = isSprinting;
        OnDeactivateCallback = onDeactivateCallback;
        OnActivateCallback = onActivateCallback;
        OnStateCompletedCallback = onStateCompletedCallback;
    }

    public Enemy Enemy { get; private set; }
    public bool IsSprinting { get; private set; }

    public UnityAction OnDeactivateCallback { get; private set; }
    public UnityAction OnActivateCallback { get; private set; }
    public UnityAction OnStateCompletedCallback { get; private set; }
}
