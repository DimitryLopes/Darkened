using UnityEngine;
using System;
using System.Collections.Generic;
public class NodeUtils : MonoBehaviour
{
    public const float NODE_SIZE = 0.975f;

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

    public static Cardinal GetCardinalDirection(MazeNode fromNode, MazeNode toNode)
    {
        Coordinate offset = new Coordinate(fromNode.X - toNode.X, fromNode.Y - toNode.Y);
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

        Debug.Log($"[{fromNode.X},{fromNode.Y}] is at {direction} of [{toNode.X},{toNode.Y}]");
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

    public static MazeWall GetRandomWall(MazeNode node, bool edgeOnly = false)
    {
        List<Cardinal> cardinals = EnumUtils.GetEnumValues<Cardinal>();
        cardinals.Shuffle();
        foreach (Cardinal cardinal in cardinals)
        {
            if (node.HasWall(cardinal) && (!edgeOnly || node.GetEdge(cardinal)))
            {
                if (edgeOnly)
                {
                    Debug.Log("Got edge wall at " + cardinal.ToString());
                }
                return node.GetWall(cardinal);
            }
        }
        return null;
    }
}
