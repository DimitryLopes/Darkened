using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using Zenject;
using Unity.VisualScripting;

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
    [SerializeField]
    private Transform itemsContainer;

    private List<MazeWall> instantiatedWalls = new List<MazeWall>();
    private List<MazeNode> instantiatedNodes = new List<MazeNode>();

    private void ClearMaze()
    {
        foreach (MazeNode node in instantiatedNodes)
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
                yield return null;
            }
        }
        CreatePath(nodes, data);
    }

    private void CreatePath(MazeNode[,] nodes, MazeData data)
    {
        MazeNode startNode = SetStartingPoint(nodes, data);
        mazeManager.CurrentStartingNode = startNode;

        Stack<MazeNode> stack = new Stack<MazeNode>();
        stack.Push(nodes[startNode.X, startNode.Y]);
        StartCoroutine(CreatePath(data, stack, nodes));
    }

    private IEnumerator CreatePath(MazeData data, Stack<MazeNode> stack, MazeNode[,] nodes)
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

        RemoveDeadEnds(data, nodes);
    }

    public void RemoveDeadEnds(MazeData data, MazeNode[,] nodes)
    {
        foreach (MazeNode node in instantiatedNodes)
        {
            if (node.IsActive && node.ActiveWalls == 3)
            {
                RemoveDeadEndWall(node, data);
            }
        }
        AddItems(data, nodes);
    }
    private void AddItems(MazeData data, MazeNode[,] nodes)
    {
        List<MissionItem> items = mazeManager.GetMissionItems(data.Objective);
        foreach (MissionItem item in items)
        {
            PositionItem(item, nodes, data);
        }
        AddTorches(data, nodes);
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
        if (neighbor != null && neighbor.Visited == false)
        {
            return neighbor;
        }
        return null;
    }

    private MazeNode GetNodeAt(Coordinate coordinate)
    {
        foreach (MazeNode node in instantiatedNodes)
        {
            if (node.X == coordinate.X && node.Y == coordinate.Y)
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
                if (node.Coordinates.Y < data.Height - 1)
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

    private void RemoveWallsAt(MazeNode nodeA, MazeNode nodeB)
    {
        Debug.Log("Removing walls between [" + nodeA.X + "," + nodeA.Y + "] and [" + nodeB.X + "," + nodeB.Y + "]");
        Cardinal direction = NodeUtils.GetCardinalDirection(nodeA, nodeB);
        Cardinal oppositeDirection = NodeUtils.GetOppositeCardinal(direction);
        nodeA.RemoveWall(oppositeDirection);

        nodeB.RemoveWall(direction);
    }

    private void RemoveWall(MazeNode node, Cardinal direction)
    {
        node.RemoveWall(direction);
    }

    private void RemoveDeadEndWall(MazeNode node, MazeData data)
    {
        List<Cardinal> cardinals = EnumUtils.GetEnumValues<Cardinal>();

        while (cardinals.Count > 0)
        {
            if (!node.GetEdge(cardinals[0]) && node.HasWall(cardinals[0]))
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

    private MazeNode GetClosestNodeToVector(Vector2 position)
    {
        float closest = float.MaxValue;
        MazeNode closestNode = null;
        foreach (MazeNode node in instantiatedNodes)
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

    private void SetNodeEdges(MazeNode node, MazeData data)
    {
        if (node.Coordinates.X == 0)
        {
            node.SetEdge(Cardinal.West, true);
        }
        else
        {
            node.SetEdge(Cardinal.West, false);
        }

        if (node.Coordinates.X == data.Width - 1)
        {
            node.SetEdge(Cardinal.East, true);
        }
        else
        {
            node.SetEdge(Cardinal.East, false);
        }

        if (node.Coordinates.Y == 0)
        {
            node.SetEdge(Cardinal.South, true);
        }
        else
        {
            node.SetEdge(Cardinal.South, false);
        }

        if (node.Coordinates.Y == data.Height - 1)
        {
            node.SetEdge(Cardinal.North, true);
        }
        else
        {
            node.SetEdge(Cardinal.North, false);
        }
    }

    private void PositionNode(MazeNode node)
    {
        node.transform.localPosition = new Vector2(node.X, node.Y);
    }

    private void PositionItem(Item item, MazeNode[,] nodes, MazeData data)
    {
        Transform transform = null;
        
        switch (item.GenerationData.SpawnType)
        {
            case SpawnType.Node:
                transform = GetAvailableNodeAwayFrom(mazeManager.CurrentStartingNode, nodes, item.GenerationData.MinDistanceFromStart).transform;
                break;
            case SpawnType.Wall:
                transform = GetAvailableWallAwayFrom(mazeManager.CurrentStartingNode, nodes, item.GenerationData.MinDistanceFromStart).transform;
                break;
            case SpawnType.EdgeWalls:
                List<MazeNode> edgeNodes = MazeUtils.GetEdgeNodes(nodes, data);
                MazeWall chosenWall = GetAvailableWallAwayFrom(mazeManager.CurrentStartingNode, edgeNodes, item.GenerationData.MinDistanceFromStart, true, item.GenerationData.Replace);
                if (chosenWall != null)
                {
                    transform = chosenWall.transform;
                }
                else
                {
                    chosenWall = GetAvailableWallAwayFrom(mazeManager.CurrentStartingNode, edgeNodes, 0, true, item.GenerationData.Replace);
                    transform = chosenWall.transform;
                }
                break;
        }
        item.transform.SetParent(itemsContainer);
        item.transform.position = transform.position;
        item.transform.rotation = transform.rotation;
    }

    #region Torches
    private void AddTorches(MazeData data, MazeNode[,] nodes)
    {
        int torchCount = 0;
        float breakChance = 0f;
        nodes.Shuffle();
        StartCoroutine(GenerateTorches(data, nodes, torchCount, breakChance));
    }

    private IEnumerator GenerateTorches(MazeData data, IEnumerable nodes, int torchCount, float breakChance)
    {
        List<MazeNode> remainingNodes = new List<MazeNode>();
        remainingNodes.AddRange(nodes);
        bool broken = false;

        foreach (MazeNode node in nodes)
        {
            float randomTorchValue = UnityEngine.Random.Range(0f, 1f);
            float randomBreakChance = UnityEngine.Random.Range(0f, 1f);
            Debug.Log("Torches: " + torchCount + "| BreakChance: " + breakChance + " | RNG: " + randomBreakChance);
            if (torchCount < data.MinTorchCount || breakChance < randomBreakChance)
            {
                if (randomTorchValue >= data.TorchRatio)
                {
                    if (node.HasAnyWall())
                    {
                        PlaceTorchAt(node, data);
                        torchCount++;

                        if (torchCount == data.MaxTorchCount)
                        {
                            break;
                        }
                        if (torchCount >= data.MinTorchCount)
                        {
                            breakChance = (float)(torchCount - data.MinTorchCount) / (data.MaxTorchCount - data.MinTorchCount);
                        }
                    }
                    remainingNodes.Remove(node);
                }
            }
            else
            {
                broken = true;
                break;
            }

            yield return null;
        }

        if (torchCount < data.MaxTorchCount && !broken)
        {
            yield return StartCoroutine(GenerateTorches(data, remainingNodes, torchCount, breakChance));
        }
    }

    public void PlaceTorchAt(MazeNode node, MazeData data)
    {
        MazeTorch torch = mazeManager.GetMazeTorch(data);
        node.AddTorch(torch);
    }
    #endregion

    #region Pooling
    private MazeWall GetAvailableWall()
    {
        foreach (MazeWall wall in instantiatedWalls)
        {
            if (!wall.IsActive)
            {
                wall.Activate();
                return wall;
            }
        }
        MazeWall newWall = Instantiate(wallPrefab, wallsContainer);
        instantiatedWalls.Add(newWall);
        newWall.Activate();
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

    #region Utils

    private MazeNode GetAvailableNodeAwayFrom(MazeNode focusNode, IEnumerable nodes, int minDistance)
    {
        List<MazeNode> candidateNodes = new List<MazeNode>();
        //TODO: make it so it picks a random node from the nodes and then check. return it after;
        //NOTE: Idk how to do that
        foreach (MazeNode node in nodes)
        {
            int distance = Math.Abs(focusNode.X - node.X) + Math.Abs(focusNode.Y - node.Y);
            Debug.Log("[" + focusNode.X + "," + focusNode.Y + "] is " + distance + " nodes distant from [" + node.X + "," + node.Y+ "]");
            if (distance >= minDistance)
            {
                candidateNodes.Add(node);
                Debug.Log("[" + focusNode.X + "," + focusNode.Y + "]" + " is far enough from " + "[" + node.X + "," + node.Y+ "]");
            }
        }

        if (candidateNodes.Count > 0)
        {
            return candidateNodes.GetRandom();
        }
        return null;
    }

    private MazeWall GetAvailableWallAwayFrom(MazeNode focusNode, IEnumerable nodes, int minDistance, bool edge = false, bool destroyWall = false)
    {
        MazeWall wall;
        MazeNode availableNode = GetAvailableNodeAwayFrom(focusNode, nodes, minDistance);
        if (edge)
        {
            wall = availableNode.GetRandomWallOnEdge();
        }
        else
        {
            wall = availableNode.GetRandomWall();
        }
        //TODO: if we manage to take this outside the method would be great
        if (destroyWall)
        {
            RemoveWall(availableNode, wall.AlignedWith);
        }

        return wall;
    }
    #endregion
}
