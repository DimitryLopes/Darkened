using System.Collections.Generic;
using UnityEngine;

public class WanderingEnemyState : EnemyState<BaseEnemyStateData>
{
    protected MazeNode currentTarget;
    protected MazeNode closestNode;

    protected Stack<MazeNode> path;

    public override void OnActivate()
    {
        SetPath();
    }

    private void SetclosestNode()
    {
        closestNode = MazeUtils.GetClosestNodeToVector(Enemy.transform.position, Data.Maze.Nodes);
    }

    public virtual void SetPath(MazeNode target = null)
    {
        SetclosestNode();
        currentTarget = target != null ? target : Data.Maze.Nodes.GetRandom();
        Debug.Log($"Enemy is going to: [{currentTarget.Coordinates.X}|{currentTarget.Coordinates.Y}]");
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
            Deactivate();
        }
    }

    protected void HandleMovement()
    {
        MazeNode nextNode = path.Peek();
        Vector3 targetDirection = nextNode.transform.position;
        Vector3 direction = (targetDirection - Enemy.transform.position).normalized;

        Enemy.Move(direction);
        float distance = Vector3.Distance(Enemy.transform.position, nextNode.transform.position);
        if (distance < 0.35f)
        {
            path.Pop();
        }
    }

    public override void HandleState()
    {
        MoveTowardsTarget();
    }
}
