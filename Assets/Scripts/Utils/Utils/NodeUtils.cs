using UnityEngine;
using System.Collections.Generic;
public class NodeUtils : MonoBehaviour
{
    public const float NODE_SIZE = 0.16f;

    private static readonly Dictionary<Cardinal, (Cardinal, Cardinal)> adjacentDirections =
        new Dictionary<Cardinal, (Cardinal, Cardinal)>
    {
        { Cardinal.North, (Cardinal.West, Cardinal.East) },
        { Cardinal.East, (Cardinal.North, Cardinal.South) },
        { Cardinal.South, (Cardinal.East, Cardinal.West) },
        { Cardinal.West, (Cardinal.South, Cardinal.North) }
    };

    public static (Cardinal, Cardinal) GetAdjacentCardinals(Cardinal direction)
    {
        return adjacentDirections[direction];
    }

    public static Vector2 GetWallPositionOffset(Cardinal direction)
    {
        Vector2 offset = Vector2.zero;

        switch (direction)
        {
            case Cardinal.North:
                offset = Vector2.up;
                break;
            case Cardinal.East:
                offset = Vector2.right;
                break;
            case Cardinal.South:
                offset = Vector2.down;
                break;
            case Cardinal.West:
                offset = Vector2.left;
                break;
        }

        return offset;
    }

    public static float GetWallRotationByCardinal(Cardinal direction)
    {
        float rotation = 0f;

        switch (direction)
        {
            case Cardinal.North:
                rotation = -90f;
                break;
            case Cardinal.East:
                rotation = 180f;
                break;
            case Cardinal.South:
                rotation = 90f;
                break;
            case Cardinal.West:
                rotation = 0f;
                break;
        }

        return rotation;
    }

    public static Cardinal GetCardinalDirection(Node fromNode, Node toNode)
    {
        if(fromNode == null || toNode == null)
        {
                       Debug.LogError("Cannot determine cardinal direction from or to a null node.");
            return Cardinal.North;
        }
        int x = fromNode.X - toNode.X;
        int y = fromNode.Y - toNode.Y;
        Coordinate offset = new Coordinate(x,y);
        Cardinal direction = Cardinal.North;
        if (offset.Y > 0 && Mathf.Abs(offset.Y) > Mathf.Abs(offset.X))
        {
            direction = Cardinal.North;
        }
        else if (offset.X > 0 && Mathf.Abs(offset.X) > Mathf.Abs(offset.Y))
        {
            direction = Cardinal.East;
        }
        else if (offset.Y < 0 && Mathf.Abs(offset.Y) > Mathf.Abs(offset.X))
        {
            direction = Cardinal.South;
        }
        else if (offset.X < 0 && Mathf.Abs(offset.X) > Mathf.Abs(offset.Y))
        {
            direction = Cardinal.West;
        }

        return direction;
    }

    public static Cardinal GetOppositeCardinal(Cardinal cardinal)
    {
        switch (cardinal)
        {
            case Cardinal.North:
                return Cardinal.South;
            case Cardinal.South:
                return Cardinal.North;
            case Cardinal.East:
                return Cardinal.West;
            case Cardinal.West:
                return Cardinal.East;
        }
        return Cardinal.North;
    }

    public static (MazeWall, Cardinal) GetAnyDefaultWall(Node node, bool edgeOnly = false)
    {
        List<Cardinal> cardinals = EnumUtils.GetEnumValues<Cardinal>();
        cardinals.Shuffle();
        foreach (Cardinal cardinal in cardinals)
        {
            if (node.HasWall(cardinal) && (!edgeOnly || node.IsOnEdge(cardinal)))
            {
                MazeWall wall = node.GetWall(cardinal);
                if (wall.GetType().IsSubclassOf(typeof(MazeWall)))
                    continue;

                return (node.GetWall(cardinal),cardinal);
            }
        }
        return (null, Cardinal.North);
    }

    public static MazeWall GetWallAt(Node node, Cardinal direction, Maze maze)
    {
        MazeWall wall = node.GetWall(direction);
        if (wall == null)
        {
            Debug.LogWarning($"No wall found at {direction} for node {node.Coordinate}. Attempting to find neighbor node.");
            var neighborNode = MazeUtils.GetNodeAtCardinalFromNode(direction, node, maze);
            var oppositeCardinal = GetOppositeCardinal(direction);
            wall = neighborNode.GetWall(oppositeCardinal);
            if (wall == null)
            {
                Debug.LogError($"No wall found at {direction} in direction {direction}. Item will not be placed.");
                return null;
            }
        }
        return wall;
    }
}
