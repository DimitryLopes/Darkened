using System;
using System.Collections.Generic;
using UnityEngine;

public class MazeUtils 
{
    public static void ExecuteActionWithAllCardinals(Action<Cardinal> action)
    {
        foreach (Cardinal cardinal in Enum.GetValues(typeof(Cardinal)))
        {
            action.Invoke(cardinal);
        }
    }

    public static void ExecuteActionWithAllCardinals(Action<Cardinal, MazeNode> action, MazeNode node)
    {
        foreach (Cardinal cardinal in Enum.GetValues(typeof(Cardinal)))
        {
            action.Invoke(cardinal, node);
        }
    }

    public static void ExecuteActionWithAllCardinals(Action<Cardinal, MazeData, MazeNode> action, MazeNode data, MazeData node)
    {
        foreach (Cardinal cardinal in Enum.GetValues(typeof(Cardinal)))
        {
            action.Invoke(cardinal, node, data);
        }
    }

    public static void ExecuteActionWithAllCardinals(Func<Cardinal, MazeNode, MazeData, MazeNode> action, MazeNode node, MazeData data, ref List<MazeNode> nodes)
    {
        foreach (Cardinal cardinal in Enum.GetValues(typeof(Cardinal)))
        {
            nodes.Add(action.Invoke(cardinal, node, data));
        }
    }

    public static List<MazeNode> GetEdgeNodes(List<MazeNode> nodes, int gridHeight, int gridWidth)
    {
        List<MazeNode> edgeNodes = new List<MazeNode>();

        foreach (MazeNode node in nodes)
        {
            if (node.X == 0 || node.X == gridWidth - 1 ||
                node.Y == 0 || node.Y == gridHeight - 1)
            {
                edgeNodes.Add(node);
            }
        }

        return edgeNodes;
    }

    public static bool IsNodeAtEdge(Coordinate coordinate, MazeData data)
    {
        return coordinate.X == 0 || coordinate.X == data.Width - 1 ||
               coordinate.Y == 0 || coordinate.Y == data.Height - 1;
    }
}
