using UnityEngine;

public class ChasingEnemyState : EnemyState<BaseEnemyStateData>
{
    protected Transform currentTarget;

    public void SetPath(Transform targetTransform)
    {
        currentTarget = targetTransform;
    }

    public override void HandleState()
    {
        Enemy.Move(currentTarget.position, true);
    }
}
