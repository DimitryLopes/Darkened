using System.Collections.Generic;
using UnityEngine;

public class MovingTowardsTargetEnemyState : EnemyState<BaseEnemyStateData>
{
    protected MazeNode currentTarget;
    protected MazeNode closestNode;

    protected Stack<MazeNode> path;

    public MovingTowardsTargetEnemyState(BaseEnemyStateData data) : base(data)
    {
    }

    private void SetclosestNode()
    {
        closestNode = MazeUtils.GetClosestNodeToVector(Enemy.transform.position, Data.Maze.Nodes);
    }

    public virtual void SetPath(Color debugColor, MazeNode target = null)
    {
        SetclosestNode();
        currentTarget = target != null ? target : Data.Maze.Nodes.GetRandom();
        currentTarget.DebugColor(debugColor);
        path = MazeUtils.GetPathFromNodeToNode(closestNode, currentTarget, Data.Maze);
        path.Pop();
    }

    protected virtual void MoveTowardsTarget()
    {
        if (path != null && path.Count > 0)
        {
            HandleMovement();
        }
        else
        {
            Complete();
        }
    }

    protected void HandleMovement()
    {
        MazeNode nextNode = path.Peek();
        Vector3 targetDirection = nextNode.transform.position;
        Vector3 direction = targetDirection - Enemy.transform.position;

        Enemy.Move(direction, Data.IsSprinting);
        float distance = Vector3.Distance(Enemy.transform.position, nextNode.transform.position);
        if (distance < 0.25f)
        {
            path.Pop();
        }
    }

    public override void HandleState()
    {
        MoveTowardsTarget();
    }
}
