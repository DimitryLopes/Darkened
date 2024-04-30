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

    public static void ExecuteActionWithAllCardinals(Action<Cardinal, LevelData, MazeNode> action, MazeNode data, LevelData node)
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

    public static List<MazeNode> GetEdgeNodes(MazeNode[,] nodes, LevelData data)
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

    public static bool IsNodeAtCorner(MazeNode nodes, LevelData data)
    {
        bool bottomLeft = nodes.X == 0 && nodes.Y == 0;
        bool topLeft = nodes.X == 0 && nodes.Y == data.Height - 1;
        bool bottomRight = nodes.X == data.Width - 1 && nodes.Y == 0;
        bool topRight = nodes.X == data.Width - 1 && nodes.Y == data.Height - 1;
        return bottomLeft || bottomRight || topLeft || topRight;
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

            List<MazeNode> neighbors = GetNeighborNodes(current, maze);
            foreach (MazeNode neighbor in neighbors)
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

        MazeNode GetLowestFScoreNode(HashSet<MazeNode> openSet)
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
    }

    public static MazeNode GetClosestNodeToVector(Vector2 position, MazeNode[,] nodes)
    {
        float closest = float.MaxValue;
        MazeNode closestNode = null;
        foreach (MazeNode node in nodes)
        {
            if (node.IsActive)
            {
                float currentDistance = Vector2.Distance(position, node.transform.position);
                if (currentDistance < closest)
                {
                    closestNode = node;
                    closest = currentDistance;
                }
            }
        }
        return closestNode;
    }

    public static int GetManhatthanDistanceFromNodeToNode(MazeNode from, MazeNode to)
    {
        int distance = Math.Abs(from.X - to.X) + Math.Abs(from.Y - to.Y);
        return distance;
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
                    return maze.NodesByCoordinate[coordinate];
                }
                break;
            case Cardinal.South:
                if (node.Coordinates.Y > 0)
                {
                    coordinate = new Coordinate(node.Coordinates.X, node.Coordinates.Y - 1);
                    return maze.NodesByCoordinate[coordinate];
                }
                break;
            case Cardinal.East:
                if (node.Coordinates.X < maze.Data.Width - 1)
                {
                    coordinate = new Coordinate(node.Coordinates.X + 1, node.Coordinates.Y);
                    return maze.NodesByCoordinate[coordinate];
                }
                break;
            case Cardinal.West:
                if (node.Coordinates.X > 0)
                {
                    coordinate = new Coordinate(node.Coordinates.X - 1, node.Coordinates.Y);
                    return maze.NodesByCoordinate[coordinate];
                }
                break;
        }
        return null;
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
            if (!node.HasWall(cardinal) && !node.GetEdge(cardinal))
            {
                MazeNode neighbour = GetNodeAtCardinalFromNode(cardinal, node, maze);
                availableNodes.Add(neighbour);
            }
        }
    }
}
