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

    public static void ExecuteActionWithAllCardinals(Action<Cardinal, RandomLevelData, MazeNode> action, MazeNode data, RandomLevelData node)
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

    public static void ExecuteActionWithAllNodes(Action<MazeNode> action, MazeNode[,] nodes)
    {
        foreach(MazeNode node in nodes)
        {
            action.Invoke(node);
        }
    }

    public static Dictionary<Coordinate, MazeNode> GetEdgeNodes(MazeNode[,] nodes, MazeSizeData data)
    {
        Dictionary<Coordinate, MazeNode> edgeNodes = new Dictionary<Coordinate, MazeNode>();
        int height = data.Height;
        int width = data.Width;

        for (int y = 0; y < height; y++)
        {
            edgeNodes.Add(nodes[0, y].Coordinates, nodes[0, y]);
            edgeNodes.Add(nodes[width - 1, y].Coordinates, nodes[width - 1, y]);
        }

        for (int x = 1; x < width - 1; x++)
        {
            edgeNodes.Add(nodes[x, 0].Coordinates, nodes[x, 0]);
            edgeNodes.Add(nodes[x, height - 1].Coordinates, nodes[x, height - 1]);
        }

        return edgeNodes;
    }

    public static Dictionary<Coordinate,MazeNode> GetCornerNodes(MazeNode[,] nodes, MazeSizeData data)
    {
        Dictionary<Coordinate, MazeNode> corners = new Dictionary<Coordinate, MazeNode>
        {
            { nodes[0, 0].Coordinates, nodes[0, 0] },
            { nodes[0, data.Height - 1].Coordinates, nodes[0, data.Height - 1] },
            { nodes[data.Width - 1, 0].Coordinates, nodes[data.Width - 1, 0] },
            { nodes[data.Width - 1, data.Height - 1].Coordinates, nodes[data.Width - 1, data.Height - 1] }
        };
        return corners;
    }

    public static int GetCantorPairing(Coordinate item1, Coordinate item2)
    {
        //use both nodes to create a pairing
        int firstPair = GetCantorPairing(item1);
        int secondPair = GetCantorPairing(item2);

        int lowest;
        int highest;
        if(firstPair < secondPair)
        {
            lowest = firstPair;
            highest = secondPair;
        }
        else
        {
            lowest = secondPair;
            highest = firstPair;
        }

        int pairing = GetCantorPairing(new Coordinate(lowest, highest));
        return pairing;
    }

    public static int GetCantorPairing(Coordinate coordinate)
    {
        int x = coordinate.X + 1;
        int y = coordinate.Y + 1;
        return (x + y) * (x + y + 1) / 2 + y;
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
                    if(maze.NodesByCoordinate.ContainsKey(coordinate))
                        return maze.NodesByCoordinate[coordinate];
                    else
                        Debug.LogWarning($"Node at {coordinate} not found in maze nodes dictionary.");
                }
                break;
        }
        return null;
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
    public static (Cardinal xDirection, Cardinal yDirection) GetCardinalDirections(Vector2 fromPosition, Vector2 toPosition)
    {
        Cardinal xDirection;
        Cardinal yDirection;

        if (toPosition.x < fromPosition.x)
        {
            xDirection = Cardinal.West;
        }
        else
        {
            xDirection = Cardinal.East;
        }

        if (toPosition.y > fromPosition.y)
        {
            yDirection = Cardinal.North;
        }
        else
        {
            yDirection = Cardinal.South;
        }

        return (xDirection, yDirection);
    }
}
