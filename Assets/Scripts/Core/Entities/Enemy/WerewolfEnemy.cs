using UnityEngine;

public class WerewolfEnemy : EnemyBase
{
    private Maze maze;
    private WanderingEnemyState wanderingState;
    private InvestigantingEnemyState investigatingState;

    public WerewolfEnemy(MazeManager mazeManager)
    {
        maze = mazeManager.CurrentMaze;

        BaseEnemyStateData wanderingStateData = new BaseEnemyStateData(mazeManager.CurrentMaze, this);
        wanderingState.SetUp(wanderingStateData);
        investigatingState.SetUp(wanderingStateData);

        investigatingState.SetOnDeactivateCallback(OnInvestigationEnded);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == Constants.Tags.PLAYER_LAYER)
        {
            investigatingState.SetPath(MazeUtils.GetClosestNodeToVector(collision.transform.position, maze.Nodes));
            ChangeState(investigatingState);
        }
    }

    #region Callbacks
    public void OnInvestigationEnded()
    {
        ChangeState(wanderingState);
    }
    #endregion
}
