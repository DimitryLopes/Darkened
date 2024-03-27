using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderingEnemyState : EnemyState<WanderingEnemyStateData>
{
    public LayerMask obstacleMask; 
    private MazeNode currentTarget;
    private Stack<Vector3> path;

    private Vector3 targetPosition;

    public override void OnActivate()
    {
        GetRandomPath();
    }

    private void GetRandomPath()
    {
        currentTarget = Data.Maze.Nodes.GetRandom();
        //path = MazeUtils.GetPathFromNodeToNode(enemy.transform.position, currentTarget);
    }

    private void MoveTowardsTarget()
    {
        if (path != null && path.Count > 0)
        {
            Vector3 nextPos = path.Peek();
            Vector3 direction = (nextPos - enemy.transform.position).normalized;
            //enemy.transform.Translate(direction * enemy.MovementSpeed * Time.deltaTime);

            if (Vector3.Distance(enemy.transform.position, nextPos) < 0.1f)
            {
                path.Pop();
            }
        }
        else
        {
            GetRandomPath();
        }
    }

    public override void HandleState()
    {
        MoveTowardsTarget();
    }
}
