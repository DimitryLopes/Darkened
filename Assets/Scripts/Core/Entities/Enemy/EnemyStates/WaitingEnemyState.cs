using UnityEngine;

public class WaitingEnemyState : State<WaitingStateData>
{
    private float timeRemaining;

    public WaitingEnemyState(WaitingStateData data) : base(data)
    {
    }

    public override void OnActivate()
    {
        base.OnActivate();
        timeRemaining = Data.WaitingTime;
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
