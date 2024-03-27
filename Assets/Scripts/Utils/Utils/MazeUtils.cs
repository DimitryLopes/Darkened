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

    public static void ExecuteActionWithAllCardinals(Func<Cardinal, MazeNode, MazeNode> action, MazeNode node, ref List<MazeNode> nodes)
    {
        foreach (Cardinal cardinal in Enum.GetValues(typeof(Cardinal)))
        {
            nodes.Add(action.Invoke(cardinal, node));
        }
    }

    public static List<MazeNode> GetEdgeNodes(MazeNode[,] nodes, MazeData data)
    {
        List<MazeNode> edgeNodes = new List<MazeNode>();
        int height = data.Height;
        int width = data.Width;

        foreach (MazeNode node in nodes)
        {
            if (node.X == 0 || node.X == width - 1 ||
                node.Y == 0 || node.Y == height - 1)
            {
                edgeNodes.Add(node);
            }
        }

        return edgeNodes;
    }

    public static Stack<MazeNode> GetPathFromNodeToNode(MazeNode startNode, MazeNode targetNode, Maze maze)
    {
        HashSet<MazeNode> openSet = new HashSet<MazeNode>();
        HashSet<MazeNode> closedSet = new HashSet<MazeNode>();
        Dictionary<MazeNode, MazeNode> cameFrom = new Dictionary<MazeNode, MazeNode>();

        openSet.Add(startNode);
        startNode.gScore = 0;
        startNode.fScore = startNode.GetHeuristic(targetNode);

        while (openSet.Count > 0)
        {
            MazeNode current = GetLowestFScoreNode(openSet);
            if (current == targetNode)
            {
                return ReconstructPath(cameFrom, current);
            }

            openSet.Remove(current);
            closedSet.Add(current);

            foreach (MazeNode neighbor in GetNeighborNodes(current, maze))
            {
                if (closedSet.Contains(neighbor))
                {
                    continue;
                }

                float tentativeGScore = current.gScore + Vector3.Distance(current.transform.position, neighbor.transform.position);

                if (!openSet.Contains(neighbor) || tentativeGScore < neighbor.gScore)
                {
                    cameFrom[neighbor] = current;
                    neighbor.gScore = tentativeGScore;
                    neighbor.fScore = neighbor.gScore + neighbor.GetHeuristic(targetNode);

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }
        return null;

        Stack<MazeNode> ReconstructPath(Dictionary<MazeNode, MazeNode> cameFrom, MazeNode currentNode)
        {
            Stack<MazeNode> path = new Stack<MazeNode>();
            path.Push(currentNode);

            while (cameFrom.ContainsKey(currentNode))
            {
                currentNode = cameFrom[currentNode];
                path.Push(currentNode);
            }

            return path;
        }
    }

    private static MazeNode GetLowestFScoreNode(HashSet<MazeNode> openSet)
    {
        MazeNode lowestNode = null;
        float lowestFScore = float.MaxValue;

        foreach (MazeNode node in openSet)
        {
            if (node.fScore < lowestFScore)
            {
                lowestNode = node;
                lowestFScore = node.fScore;
            }
        }

        return lowestNode;
    }

    public static bool IsNodeAtEdge(Coordinate coordinate, MazeData data)
    {
        return coordinate.X == 0 || coordinate.X == data.Width - 1 ||
               coordinate.Y == 0 || coordinate.Y == data.Height - 1;
    }

    public static MazeNode GetNodeAtCardinalFromNode(Cardinal direction, MazeNode node, Maze maze)
    {
        Coordinate coordinate = new();
        switch (direction)
        {
            case Cardinal.North:
                if (node.Coordinates.Y < maze.Data.Height - 1)
                {
                    coordinate = new Coordinate(node.Coordinates.X, node.Coordinates.Y + 1);
                }
                break;
            case Cardinal.South:
                if (node.Coordinates.Y > 0)
                {
                    coordinate = new Coordinate(node.Coordinates.X, node.Coordinates.Y - 1);
                }
                break;
            case Cardinal.East:
                if (node.Coordinates.X < maze.Data.Width - 1)
                {
                    coordinate = new Coordinate(node.Coordinates.X + 1, node.Coordinates.Y);
                }
                break;
            case Cardinal.West:
                if (node.Coordinates.X > 0)
                {
                    coordinate = new Coordinate(node.Coordinates.X - 1, node.Coordinates.Y);
                }
                break;
        }
        return maze.NodesByCoordinate[coordinate];
    }

    public static MazeNode GetNodeAtCoordinate(Coordinate coordinate, Maze maze)
    {
        return maze.NodesByCoordinate[coordinate];
    }

    public static List<MazeNode> GetNeighborNodes(MazeNode node, Maze maze)
    {
        List<MazeNode> availableNodes = new List<MazeNode>();
        ExecuteActionWithAllCardinals(AddToAvailableNodes);
        return availableNodes;

        void AddToAvailableNodes(Cardinal cardinal)
        {
            if (!node.HasWall(cardinal))
            {
                availableNodes.Add(GetNodeAtCardinalFromNode(Cardinal.North, node, maze));
            }
        }
    }
}
