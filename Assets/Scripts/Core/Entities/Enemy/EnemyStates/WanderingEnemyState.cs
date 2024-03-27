using System.Collections.Generic;
using UnityEngine;

public class WanderingEnemyState : EnemyState<BaseEnemyStateData>
{
    protected MazeNode currentTarget;
    protected MazeNode closestNode;

    protected Stack<MazeNode> path;

    public override void OnActivate()
    {
        SetclosestNode();
        SetPath();
    }

    private void SetclosestNode()
    {
        closestNode = MazeUtils.GetClosestNodeToVector(Enemy.transform.position, Data.Maze.Nodes);
    }

    public virtual void SetPath(MazeNode target = null)
    {
        if (target == null)
        {
            currentTarget = Data.Maze.Nodes.GetRandom();
        }
        else
        {
            currentTarget = target;
        }
        path = MazeUtils.GetPathFromNodeToNode(closestNode, currentTarget, Data.Maze);
    }

    protected virtual void MoveTowardsTarget()
    {
        if (path != null && path.Count > 0)
        {
            HandleMovement();
        }
        else
        {
            SetPath();
        }
    }

    protected void HandleMovement()
    {
        MazeNode nextNode = path.Peek();
        Vector3 targetDirection = nextNode.transform.position;
        Vector3 direction = (targetDirection - Enemy.transform.position).normalized;

        Enemy.Move(direction);

        if (Vector3.Distance(Enemy.transform.position, targetDirection) < 0.1f)
        {
            path.Pop();
        }
    }

    public override void HandleState()
    {
        MoveTowardsTarget();
    }
}
