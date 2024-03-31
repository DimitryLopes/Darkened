using UnityEngine;

public class ChasingEnemyState : EnemyState<BaseEnemyStateData>
{
    protected Transform currentTarget;
    public ChasingEnemyState(BaseEnemyStateData data) : base(data)
    {
    }

    public void SetPath(Transform targetTransform)
    {
        currentTarget = targetTransform;
    }

    public override void HandleState()
    {
        Enemy.Move(currentTarget.position - Enemy.transform.position, true);
    }
}
