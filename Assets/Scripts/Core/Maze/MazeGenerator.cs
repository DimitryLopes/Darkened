using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System;
using Zenject;

public class MazeGenerator : MonoBehaviour
{
    [Inject]
    GameManager gameManager;
    [Inject]
    private MazeManager mazeManager;

    [SerializeField]
    private MazeWall wallPrefab;
    [SerializeField]
    private MazeNode nodePrefab;

    [SerializeField, Header("Containers")]
    private Transform nodeContainer;
    [SerializeField]
    private Transform wallsContainer;

    private List<MazeWall> instantiatedWalls = new List<MazeWall>();
    private List<MazeNode> instantiatedNodes = new List<MazeNode>();

    private void ClearMaze()
    {
        foreach(MazeNode node in instantiatedNodes)
        {
            node.Deactivate();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            gameManager.StartGame();
        }
    }

    #region Main Generation
    public void CreateMaze(MazeData data)
    {
        ClearMaze();

        IEnumerator<MazeNode[,]> enumerator = CreateBase(data);
        StartCoroutine(enumerator);
    }

    private IEnumerator<MazeNode[,]> CreateBase(MazeData data)
    {
        MazeNode[,] nodes = new MazeNode[data.Width, data.Height];
        for (int y = 0; y < data.Height; y++)
        {
            for (int x = 0; x < data.Width; x++)
            {
                MazeNode newNode = GetAvailableNode();
                nodes[x, y] = newNode;
                newNode.Activate();
                newNode.SetCoordinate(x, y);
                PositionNode(newNode);
                SetNodeEdges(newNode, data);
                MazeUtils.ExecuteActionWithAllCardinals(AddNodeWalls, newNode);
                Debug.Log("Node Generated");
                yield return null;
            }
        }
        CreatePath(nodes, data);
    }

    private void CreatePath(MazeNode[,] nodes, MazeData data)
    {
        MazeNode startNode = SetStartingPoint(nodes, data);

        Stack<MazeNode> stack = new Stack<MazeNode>();
        stack.Push(nodes[startNode.X, startNode.Y]);
        StartCoroutine(CreatePath(data, stack));
    }

    private IEnumerator CreatePath(MazeData data, Stack<MazeNode> stack)
    {
        while (stack.Count > 0)
        {
            MazeNode currentNode = stack.Pop();
            Debug.Log("going to Node at: [" + currentNode.X + "," + currentNode.Y + "]");
            currentNode.Visit();
            yield return new WaitForSeconds(0);
            List<MazeNode> neighbors = GetUnvisitedNeighbors(currentNode, data);
            if (neighbors.Count > 0)
            {
                stack.Push(currentNode);
                neighbors.Shuffle();
                foreach (MazeNode node in neighbors)
                {
                    Debug.Log("Current node has node at: [" + node.X + "," + node.Y + "] as available neighbor");
                }
                stack.Push(neighbors[0]);
                RemoveWallsAt(stack.Peek(), currentNode);
            }
        }

        RemoveDeadEnds(data);
    }

    public void RemoveDeadEnds(MazeData data)
    {
        foreach(MazeNode node in instantiatedNodes)
        {
            if(node.IsActive && node.ActiveWalls == 3)
            {
                RemoveDeadEndWall(node, data);
            }
        }
        AddItems(data); 
    }
    private void AddItems(MazeData data)
    {
        List<MissionItem> items = mazeManager.GetMissionItems(data.Objective);
        foreach (MissionItem item in items)
        {
            Debug.Log("Added item");
        }
    }
    #endregion

    private List<MazeNode> GetUnvisitedNeighbors(MazeNode node, MazeData data)
    {
        List<MazeNode> neighbors = new List<MazeNode>();
        MazeUtils.ExecuteActionWithAllCardinals(AddToNeighborsList, node, data, ref neighbors);
        neighbors.RemoveAll(item => item == null);
        return neighbors;
    }

    private MazeNode AddToNeighborsList(Cardinal direction, MazeNode node, MazeData data)
    {
        MazeNode neighbor = GetNodeAtCardinalFromNode(direction, node, data);
        if(neighbor != null && neighbor.Visited == false)
        {
            return neighbor;
        }
        return null;
    }

    private MazeNode GetNodeAt(Coordinate coordinate)
    {
        foreach(MazeNode node in instantiatedNodes)
        {
            if(node.X == coordinate.X && node.Y == coordinate.Y)
            {
                return node;
            }
        }
        Debug.LogError("No node found at X: " + coordinate.X + " Y: " + coordinate.Y);
        return null;
    }

    private MazeNode GetNodeAtCardinalFromNode(Cardinal direction, MazeNode node, MazeData data)
    {
        Coordinate coordinate;
        switch (direction)
        {
            case Cardinal.North:
                if(node.Coordinates.Y < data.Height - 1)
                {
                    coordinate = new Coordinate(node.Coordinates.X, node.Coordinates.Y + 1);
                    return GetNodeAt(coordinate);
                }
                break;
            case Cardinal.South:
                if (node.Coordinates.Y > 0)
                {
                    coordinate = new Coordinate(node.Coordinates.X, node.Coordinates.Y - 1);
                    return GetNodeAt(coordinate);
                }
                break;
            case Cardinal.East:
                if (node.Coordinates.X < data.Width - 1)
                {
                    coordinate = new Coordinate(node.Coordinates.X + 1, node.Coordinates.Y);
                    return GetNodeAt(coordinate);
                }
                break;
            case Cardinal.West:
                if (node.Coordinates.X > 0)
                {
                    coordinate = new Coordinate(node.Coordinates.X - 1, node.Coordinates.Y);
                    return GetNodeAt(coordinate);
                }
                break;
        }

        return null;
    }

    private MazeNode SetStartingPoint(MazeNode[,] nodes, MazeData data)
    {
        int startingX = UnityEngine.Random.Range(0, data.Width);
        return nodes[startingX, 0];
    }

    private MazeNode SetFinishingPoint(MazeNode startingPoint, MazeData data)
    {
        MazeNode node = GetRandomEdgeCoordinate(startingPoint, data, data.MinDistanceStartToFinish);
        return node;
    }

    private void RemoveWallsAt(MazeNode nodeA, MazeNode nodeB)
    {
        Debug.Log("Removing walls between [" + nodeA.X + "," + nodeA.Y + "] and [" + nodeB.X + "," + nodeB.Y + "]");
        Cardinal direction = NodeUtils.GetCardinalDirection(nodeA, nodeB);
        Cardinal oppositeDirection = NodeUtils.GetOppositeCardinal(direction);
        nodeA.RemoveWall(oppositeDirection);

        nodeB.RemoveWall(direction);
    }
    public void RemoveDeadEndWall(MazeNode node, MazeData data)
    {
        List<Cardinal> cardinals = new List<Cardinal>();
        foreach(Cardinal cardinal in Enum.GetValues(typeof(Cardinal)))
        {
            cardinals.Add(cardinal);
        }

        while (cardinals.Count > 0)
        {
            if (!node.Edges[cardinals[0]] && node.HasWall(cardinals[0]))
            {
                MazeNode neighboorNode = GetNodeAtCardinalFromNode(cardinals[0], node, data);
                if (neighboorNode != null)
                {
                    RemoveWallsAt(node, neighboorNode);
                    break;
                }
                else
                {
                    cardinals.RemoveAt(0);
                    cardinals.Shuffle();
                }
            }
            else
            {
                cardinals.RemoveAt(0);
                cardinals.Shuffle();
            }
        }
    }

    public MazeNode GetRandomEdgeCoordinate(MazeNode node, MazeData data, float minDistance = float.MaxValue)
    {
        List<MazeNode> edges = MazeUtils.GetEdgeNodes(instantiatedNodes, data.Height, data.Width);
        int randomIndex;
        float distance;
        while (true)
        {
            randomIndex = UnityEngine.Random.Range(0, edges.Count);
            distance = Vector2.Distance(edges[randomIndex].transform.position, node.transform.position);
            if (distance <= minDistance)
            {
                return edges[randomIndex];
            }
            edges.RemoveAt(randomIndex);
        }
    }

    private MazeNode GetClosestNodeToVector(Vector2 position)
    {
        float closest = float.MaxValue;
        MazeNode closestNode = null;
        foreach(MazeNode node in instantiatedNodes)
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

    private void AddNodeWalls(Cardinal direction, MazeNode newNode)
    {
        MazeWall wall = GetAvailableWall();
        newNode.AddWall(direction, wall);
    }

    private void PositionNode(MazeNode node)
    {
        node.transform.localPosition = new Vector2(node.X, node.Y);
    }

    private void SetNodeEdges(MazeNode node, MazeData data)
    {
        if(node.Coordinates.X == 0)
        {
            node.SetEdge(Cardinal.East, true);
        }
        else
        {
            node.SetEdge(Cardinal.East, false);
        }

        if(node.Coordinates.X == data.Width - 1)
        {
            node.SetEdge(Cardinal.West, true);
        }
        else
        {
            node.SetEdge(Cardinal.West, false);
        }

        if (node.Coordinates.Y == 0)
        {
            node.SetEdge(Cardinal.South, true);
        }
        else
        {
            node.SetEdge(Cardinal.South, false);
        }

        if (node.Coordinates.Y == data.Height- 1)
        {
            node.SetEdge(Cardinal.North, true);
        }
        else
        {
            node.SetEdge(Cardinal.North, false);
        }
    }



    #region Pooling
    private MazeWall GetAvailableWall()
    {
        foreach (MazeWall wall in instantiatedWalls)
        {
            if (!wall.IsActive)
            {
                return wall;
            }
        }
        MazeWall newWall = Instantiate(wallPrefab, wallsContainer);
        instantiatedWalls.Add(newWall);
        return newWall;
    }

    private MazeNode GetAvailableNode()
    {
        foreach (MazeNode node in instantiatedNodes)
        {
            if (!node.IsActive)
            {
                return node;
            }
        }
        MazeNode newNode = Instantiate(nodePrefab, nodeContainer);
        instantiatedNodes.Add(newNode);
        return newNode;
    }
    #endregion
}
