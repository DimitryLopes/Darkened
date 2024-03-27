public class InvestigantingEnemyState : WanderingEnemyState
{
    protected override void MoveTowardsTarget()
    {
        if (path != null && path.Count > 0)
        {
            HandleMovement();
        }
        else
        {
            Deactivate();
        }
    }
}
