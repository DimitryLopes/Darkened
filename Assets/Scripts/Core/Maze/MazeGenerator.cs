using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System;
using Zenject;
using Unity.VisualScripting;

public class MazeGenerator : MonoBehaviour
{
    [Inject]
    private Coroutiner coroutiner;
    [Inject]
    private SignalBus signalBus;
    [Inject]
    private ScreenManager screenManager;

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
    [SerializeField]
    private Transform torchContainer;

    private List<MazeWall> instantiatedWalls = new List<MazeWall>();
    private List<MazeNode> instantiatedNodes = new List<MazeNode>();
    private MazeManager mazeManager;

    private MazeData currentData;
    private MazeNode[,] currentNodes;

    private void ClearMaze()
    {
        foreach (MazeNode node in instantiatedNodes)
        {
            node.Deactivate();
        }
    }

    #region Main Generation
    public void CreateMaze(MazeData data,MazeManager mazeManager)
    {
        this.mazeManager = mazeManager;
        currentData = data;
        ClearMaze();
        
        LoadingOperation mazeLoadingOperation = MazeLoadOperation();
        LoadingScreenController controller = new LoadingScreenController(mazeLoadingOperation, OnMazeGenerationFinish);
        UILoadingScreen screen = screenManager.GetScreen<UILoadingScreen>();
        screen.Show(controller);
    }

    private LoadingOperation MazeLoadOperation()
    {
        IEnumerator<float> step1enumerator = CreateBase();
        LoadingStep step1 = new LoadingStep(step1enumerator, coroutiner, "A");

        IEnumerator<float> step2enumerator = CreatePath();
        LoadingStep step2 = new LoadingStep(step2enumerator, coroutiner, "B");

        IEnumerator<float> step3enumerator = RemoveDeadEnds();
        LoadingStep step3 = new LoadingStep(step3enumerator, coroutiner, "C");

        IEnumerator<float> step4enumerator = AddItems();
        LoadingStep step4 = new LoadingStep(step4enumerator, coroutiner, "D");

        IEnumerator<float> step5enumerator = AddTorches();
        LoadingStep step5 = new LoadingStep(step5enumerator, coroutiner, "E");

        List<LoadingStep> steps = new List<LoadingStep>
        {
            step1, step2, step3, step4, step5,
        };

        LoadingOperation operation = new LoadingOperation(steps, coroutiner);
        return operation;
    }

    private IEnumerator<float> CreateBase()
    {
        MazeNode[,] nodes = new MazeNode[currentData.Width, currentData.Height];
        for (int y = 0; y < currentData.Height; y++)
        {
            for (int x = 0; x < currentData.Width; x++)
            {
                MazeNode newNode = GetAvailableNode();
                nodes[x, y] = newNode;
                newNode.Activate();
                newNode.SetCoordinate(x, y);
                PositionNode(newNode);
                SetNodeEdges(newNode, currentData);
                MazeUtils.ExecuteActionWithAllCardinals(AddNodeWalls, newNode);
                yield return LoadingUtils.GetProgress(y * currentData.Height + x, currentData.Size);
            }
        }
        currentNodes = nodes;
    }

    private Stack<MazeNode> GetNodeStack(MazeNode[,] nodes, MazeData data)
    {
        MazeNode startNode = SetStartingPoint(nodes, data);
        mazeManager.CurrentStartingNode = startNode;

        Stack<MazeNode> stack = new Stack<MazeNode>();
        stack.Push(nodes[startNode.X, startNode.Y]);
        return stack;
    }

    private IEnumerator<float> CreatePath()
    {
        float visitedNodes = 0;
        Stack<MazeNode> stack = GetNodeStack(currentNodes, currentData);
        while (stack.Count > 0)
        {
            MazeNode currentNode = stack.Pop();
            Debug.Log("going to Node at: [" + currentNode.X + "," + currentNode.Y + "]");
            currentNode.Visit();
            visitedNodes++;
            List<MazeNode> neighbors = GetUnvisitedNeighbors(currentNode, currentData);
            if (neighbors.Count > 0)
            {
                stack.Push(currentNode);
                neighbors.Shuffle();
                stack.Push(neighbors[0]);
                RemoveWallsAt(stack.Peek(), currentNode);
            }
            yield return LoadingUtils.GetProgress(visitedNodes, currentNodes.Length * 2);
        }
    }

    public IEnumerator<float> RemoveDeadEnds()
    {
        MazeNode[,] nodes = currentNodes.Clone() as MazeNode[,];

        for (int y = 0; y < currentData.Height; y++)
        {
            for (int x = 0; x < currentData.Width; x++)
            {
                if (nodes[x,y].IsActive && nodes[x,y].ActiveWalls == 3)
                {
                    StartCoroutine(RemoveDeadEndWall(nodes[x,y], currentData));
                }
                yield return LoadingUtils.GetProgress(y * currentData.Height + x, nodes.Length);
            }
        }
    }

    private IEnumerator RemoveDeadEndWall(MazeNode node, MazeData data)
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
                    yield return null;
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
            yield return null;
        }
    }

    private IEnumerator<float> AddItems()
    {
        List<MissionItem> items = mazeManager.GetMissionItems(currentData.Objective);
        for (int i = 0; i < items.Count; i++)
        {
            PositionItem(items[i]);
            yield return LoadingUtils.GetProgress(i, items.Count);
        }
    }

    private void PositionItem(Item item)
    {
        Transform transform = null;

        switch (item.GenerationData.SpawnType)
        {
            case SpawnType.Node:
                transform = GetAvailableNodeAwayFrom(mazeManager.CurrentStartingNode, currentNodes, item.GenerationData.MinDistanceFromStart).transform;
                break;
            case SpawnType.Wall:
                transform = GetAvailableWallAwayFrom(mazeManager.CurrentStartingNode, currentNodes, item.GenerationData.MinDistanceFromStart).transform;
                break;
            case SpawnType.EdgeWalls:
                List<MazeNode> edgeNodes = MazeUtils.GetEdgeNodes(currentNodes, currentData);
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


    #region Torches
    private IEnumerator<float> AddTorches()
    {
        int torchCount = 0;
        float breakChance = 0f;
        MazeNode[,] nodes = currentNodes.Clone() as MazeNode[,];
        nodes.Shuffle();

        IEnumerator<float> torchEnumerator = GenerateTorches(nodes, torchCount, breakChance);
        StartCoroutine(torchEnumerator);

        while (torchEnumerator.MoveNext())
        {
            yield return torchEnumerator.Current;
        }

        yield return 1;
    }

    private IEnumerator<float> GenerateTorches(IEnumerable nodes, int torchCount, float breakChance)
    {
        List<MazeNode> remainingNodes = new List<MazeNode>();
        remainingNodes.AddRange(nodes);
        bool broken = false;

        foreach (MazeNode node in nodes)
        {
            float randomTorchValue = UnityEngine.Random.Range(0f, 1f);
            float randomBreakChance = UnityEngine.Random.Range(0f, 1f);
            Debug.Log("Torches: " + torchCount + "| BreakChance: " + breakChance + " | RNG: " + randomBreakChance);
            if (torchCount < currentData.MinTorchCount || breakChance < randomBreakChance)
            {
                if (randomTorchValue >= currentData.TorchRatio)
                {
                    if (node.HasAnyWall())
                    {
                        PlaceTorchAt(node);
                        torchCount++;
                        if (torchCount == currentData.MaxTorchCount)
                        {
                            break;
                        }
                        if (torchCount >= currentData.MinTorchCount)
                        {
                            breakChance = (float)(torchCount - currentData.MinTorchCount) / (currentData.MaxTorchCount - currentData.MinTorchCount);
                        }
                    }
                    remainingNodes.Remove(node);
                }
                yield return LoadingUtils.GetProgress(torchCount, currentData.MaxTorchCount);
            }
            else
            {
                broken = true;
                break;
            }

            yield return LoadingUtils.GetProgress(torchCount, currentData.MaxTorchCount);
        }

        if (torchCount < currentData.MaxTorchCount && !broken)
        {
            yield return GenerateTorches(remainingNodes, torchCount, breakChance).Current;
        }
    }

    private void OnMazeGenerationFinish()
    {
        signalBus.Fire(new OnMazeLoadFinishSignal());
    }

    public void PlaceTorchAt(MazeNode node)
    {
        MazeTorch torch = mazeManager.GetMazeTorch(currentData);
        torch.transform.SetParent(torchContainer);
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
