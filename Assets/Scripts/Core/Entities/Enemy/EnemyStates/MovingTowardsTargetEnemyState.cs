using System.Collections.Generic;
using UnityEngine;

public class MovingTowardsTargetEnemyState : EnemyState<BaseEnemyStateData>
{
    protected MazeNode currentTarget;
    protected MazeNode closestNode;

    protected Stack<MazeNode> path;

    public Stack<MazeNode> Path => path;
    

    public MovingTowardsTargetEnemyState(BaseEnemyStateData data) : base(data)
    {
    }

    private void SetclosestNode(Maze maze)
    {
        closestNode = MazeUtils.GetClosestNodeToVector(Enemy.transform.position, maze.Nodes);
    }

    public virtual void SetPath(Maze maze, MazeNode target = null)
    {
        SetclosestNode(maze);
        Debug.Log("Closest Node: " + closestNode.Coordinates);
        currentTarget = target != null ? target : maze.Nodes.GetRandom();
        path = MazeUtils.GetPathFromNodeToNode(closestNode, currentTarget, maze);
        string pathString = "Path: ";
        foreach (MazeNode node in path)
        {
            pathString += "[" + node.Coordinates.X + ","+ node.Coordinates.Y + "]" + " -> ";
        }
        Debug.Log(pathString + "End");
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
