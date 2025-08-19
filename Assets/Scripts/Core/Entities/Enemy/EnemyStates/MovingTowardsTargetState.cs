using System.Collections.Generic;
using UnityEngine;

public class MovingTowardsTargetState : State<BaseStateData>
{
    protected Node currentTarget;
    protected Node closestNode;

    protected Stack<Node> path;

    public Stack<Node> Path => path;
    

    public MovingTowardsTargetState(BaseStateData data) : base(data)
    {
    }

    private void SetclosestNode(Maze maze)
    {
        closestNode = MazeUtils.GetClosestNodeToVector(Transform.transform.position, maze.Nodes);
    }

    public virtual void SetPath(Maze maze, Node target = null)
    {
        SetclosestNode(maze);
        currentTarget = target != null ? target : maze.Nodes.GetRandom();
        path = MazeUtils.GetPathFromNodeToNode(closestNode, currentTarget, maze);
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
        Node nextNode = path.Peek();
        Vector3 targetDirection = nextNode.transform.position;
        Vector3 direction = targetDirection - Transform.position;

        Data.User.Move(direction, Data.IsSprinting);
        float distance = Vector3.Distance(Transform.position, nextNode.transform.position);
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
