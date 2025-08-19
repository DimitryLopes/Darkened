using UnityEngine;

public class ChasingEnemyState : State<BaseStateData>
{
    protected Transform currentTarget;
    public ChasingEnemyState(BaseStateData data) : base(data)
    {
    }

    public void SetPath(Transform targetTransform)
    {
        currentTarget = targetTransform;
    }

    public override void HandleState()
    {
        Data.User.Move(currentTarget.position - Transform.transform.position, true);
    }
}
