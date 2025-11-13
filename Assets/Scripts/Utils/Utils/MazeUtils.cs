using System;
using System.Collections.Generic;
using System.Linq;
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

    public static void ExecuteActionWithAllCardinals(Action<Cardinal, Node> action, Node node)
    {
        foreach (Cardinal cardinal in Enum.GetValues(typeof(Cardinal)))
        {
            action.Invoke(cardinal, node);
        }
    }

    public static void ExecuteActionWithAllCardinals(Action<Cardinal, RandomLevelData, Node> action, Node data, RandomLevelData node)
    {
        foreach (Cardinal cardinal in Enum.GetValues(typeof(Cardinal)))
        {
            action.Invoke(cardinal, node, data);
        }
    }

    public static void ExecuteActionWithAllCardinals(Func<Cardinal, Node, Node> action, Node node, ref List<Node> nodes)
    {
        foreach (Cardinal cardinal in Enum.GetValues(typeof(Cardinal)))
        {
            nodes.Add(action.Invoke(cardinal, node));
        }
    }

    public static void ExecuteActionWithAllNodes(Action<Node> action, Node[,] nodes)
    {
        foreach(Node node in nodes)
        {
            action.Invoke(node);
        }
    }

    public static Dictionary<Coordinate, Node> GetEdgeNodes(Node[,] nodes, MazeSizeData data)
    {
        Dictionary<Coordinate, Node> edgeNodes = new Dictionary<Coordinate, Node>();
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

    public static Dictionary<Coordinate,Node> GetBorderNodes(Node[,] nodes, MazeSizeData data)
    {
        Dictionary<Coordinate, Node> borders = new Dictionary<Coordinate, Node>
        {
            { nodes[0, 0].Coordinates, nodes[0, 0] },
            { nodes[0, data.Height - 1].Coordinates, nodes[0, data.Height - 1] },
            { nodes[data.Width - 1, 0].Coordinates, nodes[data.Width - 1, 0] },
            { nodes[data.Width - 1, data.Height - 1].Coordinates, nodes[data.Width - 1, data.Height - 1] }
        };
        return borders;
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

    public static Stack<Node> GetPathFromNodeToNode(Node startNode, Node targetNode, Maze maze)
    {
        HashSet<Node> openSet = new HashSet<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();

        openSet.Add(startNode);
        startNode.gScore = 0;
        startNode.fScore = startNode.GetHeuristic(targetNode);

        while (openSet.Count > 0)
        {
            Node current = GetLowestFScoreNode(openSet);
            if (current == targetNode)
            {
                return ReconstructPath(cameFrom, current);
            }

            openSet.Remove(current);
            closedSet.Add(current);

            List<Node> neighbors = GetAccessibleNeighbors(current, maze);
            foreach (Node neighbor in neighbors)
            {
                if (closedSet.Contains(neighbor))
                {
                    continue;
                }

                float tentativeGScore = current.gScore + Vector3.Distance(current.Position, neighbor.Position);

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

        Stack<Node> ReconstructPath(Dictionary<Node, Node> cameFrom, Node currentNode)
        {
            Stack<Node> path = new Stack<Node>();
            path.Push(currentNode);

            while (cameFrom.ContainsKey(currentNode))
            {
                currentNode = cameFrom[currentNode];
                path.Push(currentNode);
            }

            return path;
        }

        Node GetLowestFScoreNode(HashSet<Node> openSet)
        {
            Node lowestNode = null;
            float lowestFScore = float.MaxValue;

            foreach (Node node in openSet)
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

    public static Node GetClosestNodeToVector(Vector2 position, Node[,] nodes)
    {
        float closest = float.MaxValue;
        Node closestNode = null;
        foreach (Node node in nodes)
        {
            float currentDistance = Vector2.Distance(position, node.Position);
            if (currentDistance < closest)
            {
                closestNode = node;
                closest = currentDistance;
            }
        }
        return closestNode;
    }

    public static int GetManhatthanDistanceFromNodeToNode(Node from, Node to)
    {
        int distance = Math.Abs(from.X - to.X) + Math.Abs(from.Y - to.Y);
        return distance;
    }

    public static Node GetNodeAtCardinalFromNode(Cardinal direction, Node node, Maze maze)
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

    public static List<Node> GetAccessibleNeighbors(Node node, Maze maze)
    {
        List<Node> availableNodes = new List<Node>();
        ExecuteActionWithAllCardinals(AddToAvailableNodes);
        return availableNodes;

        void AddToAvailableNodes(Cardinal cardinal)
        {
            if (!node.HasWall(cardinal) && !node.IsOnEdge(cardinal))
            {
                Node neighbour = GetNodeAtCardinalFromNode(cardinal, node, maze);
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
