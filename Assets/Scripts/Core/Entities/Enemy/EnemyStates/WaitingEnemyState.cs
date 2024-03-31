using UnityEngine;

public class WaitingEnemyState : EnemyState<WaitingEnemyStateData>
{
    private float timeRemaining;

    public WaitingEnemyState(WaitingEnemyStateData data) : base(data)
    {
    }

    public override void OnActivate()
    {
        base.OnActivate();
        timeRemaining = Data.WaitingTime;
        Enemy.Move(Vector3.zero);
    }

    public override void HandleState()
    {
        timeRemaining -= Time.deltaTime;
        if(timeRemaining <= 0)
        {
            Complete();
        }
    }
}
