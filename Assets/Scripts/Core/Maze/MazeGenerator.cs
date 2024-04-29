using System.Collections.Generic;
using UnityEngine;
using System.Collections;
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
    [Inject]
    private AudioManager audioManager;

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

    //for whatever reason injecting this directly creates a circular dependency
    private MazeManager mazeManager;

    private List<MazeWall> instantiatedWalls = new List<MazeWall>();
    private List<MazeNode> instantiatedNodes = new List<MazeNode>();

    private Maze currentMaze;
    private LevelData CurrentData => currentMaze.Data;
    private MazeNode[,] CurrentNodes => currentMaze.Nodes;

    private void ClearMaze()
    {
        foreach (MazeNode node in instantiatedNodes)
        {
            node.Deactivate();
        }
    }

    #region Main Generation
    public void CreateMaze(LevelData data, MazeManager mazeManager)
    {
        this.mazeManager = mazeManager;
        currentMaze = new Maze(data);
        ClearMaze();

        LoadingOperation mazeLoadingOperation = MazeLoadOperation();
        LoadingScreenController controller = new LoadingScreenController(mazeLoadingOperation, OnMazeGenerationFinish, audioManager);
        UILoadingScreen screen = screenManager.GetScreen<UILoadingScreen>();
        screen.Show(controller);

        signalBus.Fire(new OnMazeLoadStartedSignal(currentMaze));
    }

    private LoadingOperation MazeLoadOperation()
    {
        IEnumerator<float> step1enumerator = CreateBase();
        LoadingStep step1 = new LoadingStep(step1enumerator, coroutiner, "Creating floor");

        IEnumerator<float> step2enumerator = CreatePath();
        LoadingStep step2 = new LoadingStep(step2enumerator, coroutiner, "Generating paths");

        IEnumerator<float> step3enumerator = RemoveDeadEnds();
        LoadingStep step3 = new LoadingStep(step3enumerator, coroutiner, "Removing dead ends");

        IEnumerator<float> step4enumerator = AddItems();
        LoadingStep step4 = new LoadingStep(step4enumerator, coroutiner, "Adding items");

        IEnumerator<float> step5enumerator = AddTorches();
        LoadingStep step5 = new LoadingStep(step5enumerator, coroutiner, "Adding torches");

        IEnumerator<float> step6enumerator = ActivateMazeTorches();
        LoadingStep step6 = new LoadingStep(step6enumerator, coroutiner, "Illuminating your way");

        List<LoadingStep> steps = new List<LoadingStep>
        {
            step1, step2, step3, step4, step5, step6
        };

        LoadingOperation operation = new LoadingOperation(steps, coroutiner);
        return operation;
    }

    private IEnumerator<float> CreateBase()
    {
        MazeNode[,] nodes = new MazeNode[CurrentData.Width, CurrentData.Height];
        for (int y = 0; y < CurrentData.Height; y++)
        {
            for (int x = 0; x < CurrentData.Width; x++)
            {
                MazeNode newNode = GetAvailableNode();
                nodes[x, y] = newNode;
                newNode.Activate();
                newNode.SetCoordinate(x, y);
                PositionNode(newNode);
                newNode.DebugColor(Color.grey);
                SetNodeEdges(newNode, CurrentData);
                MazeUtils.ExecuteActionWithAllCardinals(AddNodeWalls, newNode);
                yield return LoadingUtils.GetProgress(y * CurrentData.Height + x, CurrentData.Size);
            }
        }
        SetSpawnPoints(nodes);
        nodes.Shuffle();
        currentMaze.SetNodes(nodes);
    }

    private Stack<MazeNode> GetNodeStack(MazeNode[,] nodes)
    {
        Stack<MazeNode> stack = new Stack<MazeNode>();
        stack.Push(nodes[mazeManager.CurrentStartingNode.X, mazeManager.CurrentStartingNode.Y]);

        return stack;
    }

    private void SetSpawnPoints(MazeNode[,] nodes)
    {
        MazeNode startNode = SetStartingPoint(nodes, CurrentData);
        Debug.Log($"Player spawn is: [{startNode.X}|{startNode.Y}]");
        mazeManager.CurrentStartingNode = startNode;
        startNode.MarkAsUsed();

        MazeNode enemySpawn = GetAvailableNodeAwayFrom(startNode, nodes, CurrentData.Size);
        Debug.Log($"Enemy spawn is: [{enemySpawn.X}|{enemySpawn.Y}]");
        mazeManager.EnemyStartingNode = enemySpawn;
    }

    private IEnumerator<float> CreatePath()
    {
        float visitedNodes = 0;
        Stack<MazeNode> stack = GetNodeStack(CurrentNodes);
        while (stack.Count > 0)
        {
            MazeNode currentNode = stack.Pop();
            Debug.Log("going to Node at: [" + currentNode.X + "," + currentNode.Y + "]");
            currentNode.Visit();
            visitedNodes++;
            List<MazeNode> neighbors = GetUnvisitedNeighbors(currentNode);
            if (neighbors.Count > 0)
            {
                stack.Push(currentNode);
                neighbors.Shuffle();
                stack.Push(neighbors[0]);
                RemoveWallsAt(stack.Peek(), currentNode);
            }
            yield return LoadingUtils.GetProgress(visitedNodes, CurrentNodes.Length * 2);
        }
    }

    public IEnumerator<float> RemoveDeadEnds()
    {
        MazeNode[,] nodes = CurrentNodes.Clone() as MazeNode[,];

        for (int y = 0; y < CurrentData.Height; y++)
        {
            for (int x = 0; x < CurrentData.Width; x++)
            {
                if (nodes[x,y].IsActive && nodes[x,y].ActiveWalls == 3)
                {
                    StartCoroutine(RemoveDeadEndWall(nodes[x,y]));
                }
                yield return LoadingUtils.GetProgress(y * CurrentData.Height + x, nodes.Length);
            }
        }
    }

    private IEnumerator RemoveDeadEndWall(MazeNode node)
    {
        List<Cardinal> cardinals = EnumUtils.GetEnumValues<Cardinal>();
        cardinals.Shuffle();
        int wallsRemoved = 0;
        while (cardinals.Count > 0)
        {
            if (!node.GetEdge(cardinals[0]) && node.HasWall(cardinals[0]))
            {
                MazeNode neighboorNode = MazeUtils.GetNodeAtCardinalFromNode(cardinals[0], node, currentMaze);
                if (neighboorNode != null)
                {
                    RemoveWallsAt(node, neighboorNode);
                    wallsRemoved++;
                    if (wallsRemoved == 2) break;
                    yield return null;
                }
                else
                {
                    cardinals.RemoveAt(0);
                }
            }
            else
            {
                cardinals.RemoveAt(0);
            }
            yield return null;
        }
    }

    private IEnumerator<float> AddItems()
    {
        List<MissionItem> items = mazeManager.GetMissionItems();
        for (int i = 0; i < items.Count; i++)
        {
            items[i].Activate();
            PositionItem(items[i]);
            yield return LoadingUtils.GetProgress(i, items.Count);
        }
    }

    private void PositionItem(Item item)
    {
        Transform transform = null;
        MazeNode[,] shuffledNodes = CurrentNodes.Clone() as MazeNode[,];
        shuffledNodes.Shuffle();

        switch (item.GenerationData.SpawnType)
        {
            case SpawnType.Node:
                MazeNode node = GetAvailableNodeAwayFrom(currentMaze.UsedNodes, shuffledNodes, item.GenerationData.MinDistanceFromStart);
                transform = node.transform;
                break;
            case SpawnType.Wall:
                MazeWall wall = GetAvailableWallAwayFrom(currentMaze.UsedNodes, shuffledNodes, item.GenerationData.MinDistanceFromStart);
                transform = wall.transform;
                break;
            case SpawnType.EdgeWalls:
                List<MazeNode> edgeNodes = MazeUtils.GetEdgeNodes(shuffledNodes, CurrentData);
                MazeWall chosenWall = GetAvailableWallAwayFrom(currentMaze.UsedNodes, edgeNodes, item.GenerationData.MinDistanceFromStart, true, item.GenerationData.Replace);
                if (chosenWall != null)
                {
                    transform = chosenWall.transform;
                }
                else
                {
                    chosenWall = GetAvailableWallAwayFrom(currentMaze.UsedNodes, edgeNodes, 0, true, item.GenerationData.Replace);
                    transform = chosenWall.transform;
                }
                break;
        }
        item.transform.SetParent(itemsContainer);
        item.transform.position = transform.position;
        item.transform.rotation = transform.rotation;
    }

    private void OnMazeGenerationFinish()
    {
        signalBus.Fire(new OnMazeLoadFinishSignal(currentMaze));
    }

    #endregion

    private List<MazeNode> GetUnvisitedNeighbors(MazeNode node)
    {
        List<MazeNode> neighbors = new List<MazeNode>();
        MazeUtils.ExecuteActionWithAllCardinals(AddToNeighborsList, node, ref neighbors);
        neighbors.RemoveAll(item => item == null);
        return neighbors;
    }

    private MazeNode AddToNeighborsList(Cardinal direction, MazeNode node)
    {
        MazeNode neighbor = MazeUtils.GetNodeAtCardinalFromNode(direction, node, currentMaze);
        if (neighbor != null && neighbor.Visited == false)
        {
            return neighbor;
        }
        return null;
    }


    private MazeNode SetStartingPoint(MazeNode[,] nodes, LevelData data)
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

    private void AddNodeWalls(Cardinal direction, MazeNode newNode)
    {
        MazeWall wall = GetAvailableWall();
        newNode.AddWall(direction, wall);
    }

    private void SetNodeEdges(MazeNode node, LevelData data)
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
        MazeNode[,] nodes = CurrentNodes.Clone() as MazeNode[,];
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
            float randomTorchValue = Random.Range(0f, 1f);
            float randomBreakChance = Random.Range(0f, 1f);
            Debug.Log("Torches: " + torchCount + "| BreakChance: " + breakChance + " | RNG: " + randomBreakChance);
            if (torchCount < CurrentData.MinTorchCount || breakChance < randomBreakChance)
            {
                if (randomTorchValue >= CurrentData.TorchRatio)
                {
                    if (node.HasAnyWall())
                    {
                        PlaceTorchAt(node);
                        torchCount++;
                        if (torchCount == CurrentData.MaxTorchCount)
                        {
                            break;
                        }
                        if (torchCount >= CurrentData.MinTorchCount)
                        {
                            breakChance = (float)(torchCount - CurrentData.MinTorchCount) / (CurrentData.MaxTorchCount - CurrentData.MinTorchCount);
                        }
                    }
                    remainingNodes.Remove(node);
                }
                yield return LoadingUtils.GetProgress(torchCount, CurrentData.MaxTorchCount);
            }
            else
            {
                broken = true;
                break;
            }

            yield return LoadingUtils.GetProgress(torchCount, CurrentData.MaxTorchCount);
        }

        if (torchCount < CurrentData.MaxTorchCount && !broken)
        {
            yield return GenerateTorches(remainingNodes, torchCount, breakChance).Current;
        }
    }

    private IEnumerator<float> ActivateMazeTorches()
    {
        int averageAmount = Mathf.CeilToInt(currentMaze.Torches.Count * CurrentData.DifficultyData.LitTorchRatio);
        int torchCount = 0;
        foreach (MazeTorch torch in currentMaze.Torches)
        {
            if (torch.isLit) continue;
            torch.ActivateLights();
            torchCount++;
            yield return LoadingUtils.GetProgress(torchCount, averageAmount);

            if (torchCount >= averageAmount)
            {
                break;
            }
        }


        yield return 1;
    }



    public void PlaceTorchAt(MazeNode node)
    {
        MazeTorch torch = mazeManager.GetMazeTorch(CurrentData);
        torch.transform.SetParent(torchContainer);
        currentMaze.AddTorch(node, torch);
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
        int maxDistance = 0;
        MazeNode fallbackNode = null;

        foreach (MazeNode node in nodes)
        {
            int distance = MazeUtils.GetManhatthanDistanceFromNodeToNode(focusNode, node);
            Debug.Log("[" + focusNode.X + "," + focusNode.Y + "] is " + distance + " nodes distant from [" + node.X + "," + node.Y + "]");
            if (distance >= minDistance)
            {
                Debug.Log("[" + focusNode.X + "," + focusNode.Y + "]" + " is far enough from " + "[" + node.X + "," + node.Y + "]");
                return node; // Return the first node that meets the requirement
            }
            else if(distance > maxDistance)
            {
                fallbackNode = node;
                maxDistance = distance;
            }
        }

        return fallbackNode;
    }

    private MazeNode GetAvailableNodeAwayFrom(IEnumerable focusNodes, IEnumerable nodes, int minDistance)
    {
        int closestDistance = int.MaxValue;
        MazeNode closestNode = null;

        foreach (MazeNode node in nodes)
        {
            if (node.IsBeingUsed) continue;

            int maxDistance = 0;

            foreach (MazeNode focusNode in focusNodes)
            {
                int distance = MazeUtils.GetManhatthanDistanceFromNodeToNode(focusNode, node);
                if (distance < minDistance)
                {
                    // This node is too close to at least one focus node, skip to the next node
                    maxDistance = 0;
                    break;
                }
                maxDistance = Mathf.Max(maxDistance, distance);
            }

            if (maxDistance >= minDistance)
            {
                // This node is far enough from all focus nodes
                return node;
            }

            if (maxDistance < closestDistance)
            {
                closestDistance = maxDistance;
                closestNode = node;
            }
        }

        return closestNode;
    }

    private MazeWall GetAvailableWallAwayFrom(IEnumerable focusNodes, IEnumerable nodes, int minDistance, bool edge = false, bool destroyWall = false)
    {
        MazeWall wall;
        MazeNode availableNode = GetAvailableNodeAwayFrom(focusNodes, nodes, minDistance);
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
