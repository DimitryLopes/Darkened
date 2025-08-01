using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Zenject;

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
    private Gate gatePrefab;
    [SerializeField]
    private MazeNode nodePrefab;

    [SerializeField, Header("Containers")]
    private Transform nodeContainer;
    [SerializeField]
    private Transform wallsContainer;
    [SerializeField]
    private Transform edgeWallsContainer;
    [SerializeField]
    private Transform itemsContainer;
    [SerializeField]
    private Transform torchContainer;

    [SerializeField, Header("Shadows")]
    private CompositeCollider2D mazeCollider;
    [SerializeField]
    private ShadowCasterGenerator shadowCreator;
    

    //for whatever reason injecting this directly creates a circular dependency
    private MazeManager mazeManager;

    private Dictionary<Coordinate, MazeNode> instantiatedNodes = new();
    private List<MazeNode> generationPath = new List<MazeNode>();
    private List<MazeNode> generationNeighbors = new List<MazeNode>();
    private Dictionary<string, MazeWall> walls = new();
    private List<Gate> gates = new List<Gate>();

    private Maze currentMaze;

    private LevelData CurrentData => currentMaze.Data;

    private void ClearMaze()
    {
        if(currentMaze != null)
        {
            foreach (MazeNode node in currentMaze.Nodes)
            {
                node.Deactivate();
            }
        }
        generationPath.Clear();
    }

    #region Main Generation
    public void CreateMaze(RandomLevelData data, MazeManager mazeManager)
    {
        this.mazeManager = mazeManager;
        ClearMaze();
        currentMaze = new Maze(data);

        LoadingOperation mazeLoadingOperation = MazeLoadOperation();
        LoadingScreenController controller = new LoadingScreenController(mazeLoadingOperation, OnMazeGenerationFinish, audioManager);
        UILoadingScreen screen = screenManager.GetScreen<UILoadingScreen>();
        screen.Show(controller);

        signalBus.Fire(new OnMazeLoadStartedSignal(currentMaze));
    }

    public void CreateMaze(PresetLevelData preset, MazeManager mazeManager)
    {
        this.mazeManager = mazeManager;
        ClearMaze();
        currentMaze = new Maze(preset);

        int width = preset.SizeData.Width;
        int height = preset.SizeData.Height;
        MazeNode[,] nodes = new MazeNode[width, height];

        LoadingOperation mazeLoadingOperation = MazeLoadOperationPreset(preset, nodes);
        LoadingScreenController controller = new LoadingScreenController(mazeLoadingOperation, OnMazeGenerationFinish, audioManager);
        UILoadingScreen screen = screenManager.GetScreen<UILoadingScreen>();
        screen.Show(controller);

        signalBus.Fire(new OnMazeLoadStartedSignal(currentMaze));
    }

    private LoadingOperation MazeLoadOperation()
    {
        IEnumerator<float> step1enumerator = CreateBase();
        LoadingStep step1 = new LoadingStep(step1enumerator, coroutiner, "Creating floor");

        IEnumerator<float> step2enumerator = CreateWalls();
        LoadingStep step2 = new LoadingStep(step2enumerator, coroutiner, "Creating Walls");

        IEnumerator<float> step3enumerator = CreatePath();
        LoadingStep step3 = new LoadingStep(step3enumerator, coroutiner, "Generating paths");

        //IEnumerator<float> step4enumerator = RemoveDeadEnds();
        //LoadingStep step4 = new LoadingStep(step4enumerator, coroutiner, "Removing dead ends");

        IEnumerator<float> step5enumerator = AddItems();
        LoadingStep step5 = new LoadingStep(step5enumerator, coroutiner, "Adding items");

        IEnumerator<float> step6enumerator = AddTorches();
        LoadingStep step6 = new LoadingStep(step6enumerator, coroutiner, "Adding torches");

        IEnumerator<float> step7enumerator = AddGates();
        LoadingStep step7 = new LoadingStep(step7enumerator, coroutiner, "Blocking your way");

        List<LoadingStep> steps = new List<LoadingStep>
        {
            step1, step2, step3, /*step4,*/ step5, step6//, step7
        };

        LoadingOperation operation = new LoadingOperation(steps, coroutiner);
        return operation;
    }

    private LoadingOperation MazeLoadOperationPreset(PresetLevelData preset, MazeNode[,] nodes)
    {
        IEnumerator<float> step1enumerator = CreateNodesPreset(preset, nodes);
        LoadingStep step1 = new LoadingStep(step1enumerator, coroutiner, "Creating floor");

        IEnumerator<float> step2enumerator = CreateWallsPreset(preset, nodes);
        LoadingStep step2 = new LoadingStep(step2enumerator, coroutiner, "Creating Walls");

        IEnumerator<float> step3enumerator = CreateItemsPreset(preset, nodes);
        LoadingStep step3 = new LoadingStep(step3enumerator, coroutiner, "Adding items");

        IEnumerator<float> step4enumerator = SetPositionsPreset(preset, nodes);
        LoadingStep step4 = new LoadingStep(step4enumerator, coroutiner, "Setting positions");

        List<LoadingStep> steps = new List<LoadingStep>
        {
            step1, step2, step3, step4
        };

        LoadingOperation operation = new LoadingOperation(steps, coroutiner);
        return operation;
    }

    private IEnumerator<float> CreateBase()
    {
        MazeNode[,] nodes = new MazeNode[CurrentData.Width, CurrentData.Height];
        MazeNode node;
        for (int y = 0; y < CurrentData.Height; y++)
        {
            for (int x = 0; x < CurrentData.Width; x++)
            {
                node = GetNode(new Coordinate(x,y));
                nodes[x, y] = node;
                node.Activate();
                node.SetCoordinate(x, y);
                SetNodeEdges(node, CurrentData.SizeData);
                if ((y * CurrentData.Height + x) % 10 == 0)
                    yield return LoadingUtils.GetProgress(y * CurrentData.Height + x, CurrentData.Size);
            }
        }
        SetSpawnPoints(nodes);
        currentMaze.SetNodes(nodes);
    }

    private IEnumerator<float> CreateWalls()
    {
        int lenght = currentMaze.Nodes.Length;
        MazeNode currentNode;
        for (int y = 0; y < CurrentData.Height; y++)
        {
            for (int x = 0; x < CurrentData.Width; x++)
            {
                currentNode = currentMaze.Nodes[x,y];
                MazeUtils.ExecuteActionWithAllCardinals(AddNodeWall, currentNode);
            }
            yield return LoadingUtils.GetProgress(y * CurrentData.Height, lenght);
        }
    }

    private Stack<MazeNode> GetNodeStack()
    {
        Stack<MazeNode> stack = new Stack<MazeNode>();
        Coordinate coordinate = new Coordinate(mazeManager.CurrentStartingNode.X, mazeManager.CurrentStartingNode.Y);
        stack.Push(currentMaze.NodesByCoordinate[coordinate]);

        return stack;
    }

    private void SetSpawnPoints(MazeNode[,] nodes)
    {
        MazeNode startNode = SetStartingPoint(nodes, CurrentData);
        mazeManager.CurrentStartingNode = startNode;
        currentMaze.FreeNodes.Remove(startNode);

        MazeNode enemySpawn = GetAvailableNodeAwayFrom(startNode, nodes, CurrentData.Size);
        mazeManager.EnemyStartingNode = enemySpawn;
    }

    private IEnumerator<float> CreatePath()
    {
        float visitedNodes = 0;
        Stack<MazeNode> stack = GetNodeStack();
        bool backtracking = false;
        int targetProgress = currentMaze.Nodes.Length * 2;
        List<MazeNode> neighbors;

        while (stack.Count > 0)
        {
            MazeNode currentNode = stack.Pop();
            currentNode.Visit();
            generationPath.Add(currentNode);
            visitedNodes++;
            neighbors = GetNeighbors(currentNode, true);
            if (neighbors.Count > 0)
            {
                if (backtracking)
                {
                    backtracking = false;
                }

                stack.Push(currentNode);
                neighbors.Shuffle();
                stack.Push(neighbors[0]);
                RemoveWallBetween(stack.Peek(), currentNode);
            }
            else
            {
                if (!backtracking)
                {
                    backtracking = true;
                    RemoveDeadEndWalls(currentNode);
                }
            }

            if (visitedNodes % 10 == 0)
            {
                yield return LoadingUtils.GetProgress(visitedNodes, targetProgress);
            }
        }
        yield return LoadingUtils.GetProgress(visitedNodes, targetProgress);
    }

    private void RemoveDeadEndWalls(MazeNode node)
    {
        List<Cardinal> cardinals = EnumUtils.GetEnumValues<Cardinal>();
        if (node.IsOnCorner)
        {
            List<MazeNode> cornerNeighbors = GetNeighbors(node, false);
            foreach (MazeNode corner in cornerNeighbors)
            {
                RemoveWallBetween(corner, node);
            }
            return;
        }

        int wallsRemoved = 0;
        cardinals.Shuffle();
        while (cardinals.Count > 0 && wallsRemoved < 2)
        {
            var direction = cardinals[0];
            cardinals.RemoveAt(0);

            if (!node.GetEdge(direction) && node.HasWall(direction))
            {
                MazeNode neighbor = MazeUtils.GetNodeAtCardinalFromNode(direction, node, currentMaze);
                RemoveWallBetween(node, neighbor);
                wallsRemoved++;
            }
        }
    }

    private IEnumerator<float> AddItems()
    {
        List<Item> items = mazeManager.GetMissionItems();
        int randomItemAmount = GetRandomItemAmount(CurrentData.SizeData.AverageAdditionalItemAmount);

        List<DifficultyData.DifficultyItemData> itemPool = new List<DifficultyData.DifficultyItemData>(CurrentData.DifficultyData.DifficultyItemDatas);
        itemPool.Sort((a, b) => a.Probability.CompareTo(b.Probability));


        List<(float, float)> probabilities = new();
        for (int i = 0; i < randomItemAmount; i++)
        {
            probabilities.Clear();
            float totalProbability = 0f;
            foreach (var itemData in itemPool)
            {
                float oldProbability = totalProbability;
                totalProbability += itemData.Probability;
                (float, float) range = (oldProbability, totalProbability);
                probabilities.Add(range);
            }

            ItemType AdditionalItem = GetRandomItem(totalProbability,ref probabilities, itemPool);
            items.Add(mazeManager.GetAvailableItem(AdditionalItem));
            yield return LoadingUtils.GetProgress(i, randomItemAmount * 2);
        }

        for (int i = 0; i < items.Count; i++)
        {
            items[i].Activate();
            PositionItem(items[i]);
            yield return LoadingUtils.GetProgress(i + randomItemAmount, items.Count + randomItemAmount);
        }
    }

    private int GetRandomItemAmount(float itemAverage)
    {
        int baseItemAmount = Mathf.FloorToInt(itemAverage);
        float additionalItemProbability = itemAverage - baseItemAmount;
        float randomValue = UnityEngine.Random.Range(-additionalItemProbability, additionalItemProbability);
        int items = Mathf.RoundToInt(itemAverage + randomValue);

        return items;
    }

    public ItemType GetRandomItem(float totalProbability,ref List<(float,float)> probabilities, List<DifficultyData.DifficultyItemData> itemPool)
    {
        float randomValue = UnityEngine.Random.Range(0f, totalProbability);

        for (int i = 0; i < probabilities.Count; i++)
        {
            if (randomValue >= probabilities[i].Item1 && randomValue < probabilities[i].Item2)
            {
                ItemType item = itemPool[i].Item;
                probabilities.RemoveAt(i);
                itemPool.RemoveAt(i);
                return item;
            }
        }

        // This should never happen unless there's an issue with the probabilities
        Debug.LogError("Failed to select a random item. Check probability values.");
        return default;
    }

    private void PositionItem(Item item)
    {
        Transform transform = null;
        List<MazeNode> shuffledNodes = new(currentMaze.FreeNodes);
        shuffledNodes.Shuffle();

        switch (item.GenerationData.SpawnType)
        {
            case SpawnType.Node:
                MazeNode node = GetAvailableNodeAwayFrom(currentMaze.UsedNodes, shuffledNodes, item.GenerationData.MinDistanceFromThings);
                transform = node.transform;
                break;
            case SpawnType.Wall:
                MazeWall wall = GetAvailableWallAwayFrom(currentMaze.UsedNodes, shuffledNodes, item.GenerationData.MinDistanceFromThings, false, item.GenerationData.Replace);
                transform = wall.transform;
                break;
            case SpawnType.EdgeWalls:
                List<MazeNode> edgeNodes = currentMaze.EdgeNodes.Values.ToList();
                MazeWall chosenWall = GetAvailableWallAwayFrom(currentMaze.UsedNodes, edgeNodes, item.GenerationData.MinDistanceFromThings, true, item.GenerationData.Replace);
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

    private IEnumerator<float> AddGates()
    {
        int gateCount =  Mathf.FloorToInt(currentMaze.Data.SizeData.AverageGateAmount);
        float additionalGateChance = currentMaze.Data.SizeData.AverageGateAmount - gateCount;
        float random = Random.Range(0, 1f);
        if(random < additionalGateChance)
        {
            gateCount++;
        }

        for (int i = 0; i < gateCount; i++)
        {
            MazeNode availableNode = GetAvailableNode(true);
            MazeWall wall = availableNode.GetRandomWall();

            Gate gate = GetGate();
            Switch switchItem = mazeManager.GetAvailableItem(ItemType.Switch) as Switch;
            gate.Setup(availableNode, wall.AlignedWith);
            switchItem.Associate(gate);
            PositionItem(switchItem);
            
            yield return LoadingUtils.GetProgress(i + 1, gateCount);
        }


        signalBus.Fire(new OnItemsLoadFinishSignal());
        shadowCreator.Create(mazeCollider);
        yield return 1;
    }

    private void OnMazeGenerationFinish()
    {
        signalBus.Fire(new OnMazeLoadFinishSignal(currentMaze));
    }
    #endregion

    private List<MazeNode> GetNeighbors(MazeNode node, bool excludeVisited, bool removeNulls = true)
    {
        generationNeighbors.Clear();
        MazeUtils.ExecuteActionWithAllCardinals(AddToNeighborsList, node, ref generationNeighbors);

        generationNeighbors.RemoveAll(item => (item == null && removeNulls) || (excludeVisited && item.Visited));

        return generationNeighbors;
    }


    private MazeNode AddToNeighborsList(Cardinal direction, MazeNode node)
    {
        MazeNode neighbor = MazeUtils.GetNodeAtCardinalFromNode(direction, node, currentMaze);
        if (neighbor != null)
        {
            return neighbor;
        }
        return null;
    }

    private MazeNode SetStartingPoint(MazeNode[,] nodes, LevelData data)
    {
        int startingX = UnityEngine.Random.Range(0, data.Width);
        currentMaze.MarkNodeAsUsed(nodes[startingX, 0]);
        return nodes[startingX, 0];
    }

    #region Walls
    private void RemoveWallBetween(MazeNode nodeA, MazeNode nodeB)
    {
        Cardinal direction = NodeUtils.GetCardinalDirection(nodeB, nodeA);

        RemoveWall(nodeA, direction);
    }

    public void RemoveWall(MazeNode node, Cardinal direction)
    {
        Cardinal oppositeDirection = NodeUtils.GetOppositeCardinal(direction);
        MazeNode neighbor = MazeUtils.GetNodeAtCardinalFromNode(direction, node, currentMaze);
        node.RemoveWall(direction);

        if (neighbor == null) return;

        neighbor.RemoveWall(oppositeDirection);
    }

    private void AddNodeWall(Cardinal direction, MazeNode node)
    {
        string wallID = GetWallKey(node, direction);
        MazeWall wall = GetWall(wallID);

        node.AddWall(direction, wall);

        if (!node.GetEdge(direction)) return;

        wall.transform.SetParent(edgeWallsContainer);
    }

    public void AddGate(MazeNode node, Cardinal direction, Gate gate = null)
    {
        Gate gateToAdd = gate;
        if (gate == null)
        {
            gateToAdd = GetGate();
        }

        node.AddWall(direction, gate); 
        MazeNode neighbor = MazeUtils.GetNodeAtCardinalFromNode(direction, node, currentMaze);
        if(neighbor != null)
        {
            neighbor.AddWall(NodeUtils.GetOppositeCardinal(direction), gate);
        }
    }

    private string GetWallKey(MazeNode node, Cardinal direction)
    {
        // Coordenada do nodo atual
        int x = node.Coordinates.X;
        int y = node.Coordinates.Y;

        int nx, ny;
        // Simular nodo vizinho
        switch (direction)
        {
            case Cardinal.North:
                nx = x;
                ny = y + 1;
                break;
            case Cardinal.South:
                nx = x;
                ny = y - 1;
                break;
            case Cardinal.East:
                nx = x + 1;
                ny = y;
                break;
            case Cardinal.West:
                nx = x - 1;
                ny = y;
                break;
            default:
                nx = x;
                ny = y;
                break;
        }


        // Ordena para garantir simetria
        if (x > nx || (x == nx && y > ny))
        {
            return string.Format(Constants.Generation.WALL_ID_FORMAT, nx, ny, x, y);
        }
        else
        {
            return string.Format(Constants.Generation.WALL_ID_FORMAT, x, y, nx, ny);
        }
    }
    #endregion

    private void SetNodeEdges(MazeNode node, MazeSizeData data)
    {
        if (node.Coordinates.X == 0)
        {
            node.SetEdge(Cardinal.West, true);
            if (node.Coordinates.Y == 0 || node.Coordinates.Y == data.Height - 1)
            {
                node.SetCorner();
            }
        }
        else
        {
            node.SetEdge(Cardinal.West, false);
        }

        if (node.Coordinates.X == data.Width - 1)
        {
            node.SetEdge(Cardinal.East, true); 
            if (node.Coordinates.Y == 0 || node.Coordinates.Y == data.Height -1)
            {
                node.SetCorner();
            }
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

    #region Preset Generation
    private IEnumerator<float> CreateNodesPreset(PresetLevelData preset, MazeNode[,] nodes)
    {
        int width = preset.SizeData.Width;
        int height = preset.SizeData.Height;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                MazeNode node = GetNode(new Coordinate(x, y));
                nodes[x, y] = node;
                node.Activate();
                node.SetCoordinate(x, y);
                SetNodeEdges(node, preset.SizeData);
                yield return LoadingUtils.GetProgress(y * width + x, width * height);
            }
        }
        currentMaze.SetNodes(nodes);
    }

    private IEnumerator<float> CreateWallsPreset(PresetLevelData preset, MazeNode[,] nodes)
    {
        var wallPositions = preset.WallPositions;
        int totalWalls = wallPositions.Count;
        int currentWall = 0;

        foreach (var wallData in wallPositions)
        {
            MazeNode node = nodes[wallData.Coordinate.X, wallData.Coordinate.Y];
            MazeNode neighbor = MazeUtils.GetNodeAtCardinalFromNode(wallData.Direction, node, currentMaze);

            string wallID = GetWallKey(node, wallData.Direction);
            MazeWall wall = GetWall(wallID);
            node.AddWall(wallData.Direction, wall);
            if (neighbor)
            {
                neighbor.AddWall(NodeUtils.GetOppositeCardinal(wallData.Direction), wall);
            }

            if (node.GetEdge(wallData.Direction))
                wall.transform.SetParent(edgeWallsContainer);
            else
                wall.transform.SetParent(wallsContainer);

            currentWall++;
            yield return LoadingUtils.GetProgress(currentWall, totalWalls);
        }

        signalBus.Fire(new OnItemsLoadFinishSignal());
        shadowCreator.Create(mazeCollider);
        yield return LoadingUtils.GetProgress(1, 1);
    }

    private IEnumerator<float> CreateItemsPreset(PresetLevelData preset, MazeNode[,] nodes)
    {
        var presetItemsData = new List<PresetItemData>(preset.Items);
        int totalItems = presetItemsData.Count;
        int currentItem = 0;

        List<ItemPositionData> items = mazeManager.GetPresetMissionItems(preset);

        foreach(ItemPositionData itemPositionData in items)
        {
            switch (itemPositionData.Item.GenerationData.SpawnType)
            {
                case SpawnType.Node:
                    MazeNode node = nodes[itemPositionData.PresetData.Coordinate.X, itemPositionData.PresetData.Coordinate.Y];
                    itemPositionData.Item.transform.position = node.transform.position;
                    break;
                case SpawnType.Wall:
                case SpawnType.EdgeWalls:
                    MazeNode edgeNode = nodes[itemPositionData.PresetData.Coordinate.X, itemPositionData.PresetData.Coordinate.Y];
                    MazeWall wall = NodeUtils.GetWallAt(edgeNode, itemPositionData.PresetData.Direction, currentMaze);
                    itemPositionData.Item.transform.position = wall.transform.position;
                    itemPositionData.Item.transform.rotation = wall.transform.rotation;
                    break;
            }
            presetItemsData.Remove(itemPositionData.PresetData);
        }

        foreach (var itemData in presetItemsData)
        {
            if(itemData.Item.Type == ItemType.DefaultTorch)
            {
                PlaceTorchAt(nodes[itemData.Coordinate.X, itemData.Coordinate.Y], true, itemData.Direction);
                currentItem++;
                yield return LoadingUtils.GetProgress(currentItem, totalItems);
                continue;
            }
            Item item = mazeManager.GetAvailableItem(itemData.Item.Type);
            item.transform.SetParent(itemsContainer);
            item.Activate();

            switch (itemData.Item.GenerationData.SpawnType)
            {
                case SpawnType.Node:
                    MazeNode node = nodes[itemData.Coordinate.X, itemData.Coordinate.Y];
                    item.transform.position = node.transform.position;
                    break;
                case SpawnType.Wall:
                case SpawnType.EdgeWalls:
                    MazeNode edgeNode = nodes[itemData.Coordinate.X, itemData.Coordinate.Y];
                    MazeWall wall = NodeUtils.GetWallAt(edgeNode, itemData.Direction, currentMaze);
                    item.transform.position = wall.transform.position;
                    item.transform.rotation = wall.transform.rotation;
                    break;
            }
            currentItem++;
            yield return LoadingUtils.GetProgress(currentItem, totalItems);
        }
    }

    private IEnumerator<float> SetPositionsPreset(PresetLevelData preset, MazeNode[,] nodes)
    {
        MazeNode startNode = nodes[preset.StartPosition.X, preset.StartPosition.Y];
        mazeManager.CurrentStartingNode = startNode;

        MazeNode enemySpawnNode = nodes[preset.EnemySpawnPosition.X, preset.EnemySpawnPosition.Y];
        mazeManager.EnemyStartingNode = enemySpawnNode;
        mazeManager.SetupMissions();
        yield return 1;
    }
    #endregion

    #region Torches
    //THIS ONLY WORKS WITH RANDOM GENERATION
    private IEnumerator<float> AddTorches()
    {
        int minTorchCount = (CurrentData as RandomLevelData).MinTorchCount;
        int maxTorchCount = (CurrentData as RandomLevelData).MaxTorchCount;
        int torchCount = Random.Range(minTorchCount, maxTorchCount + 1);

        // Filtra nós válidos (sem item/tocha)
        List<MazeNode> validNodes = new List<MazeNode>();
        foreach (MazeNode node in currentMaze.FreeNodes)
        {
            if (node.HasAnyWall())
                validNodes.Add(node);
        }

        if (validNodes.Count < torchCount)
            torchCount = validNodes.Count;

        validNodes.Shuffle();

        // Divide em partes para espalhar
        float step = (float)validNodes.Count / torchCount;
        int added = 0;
        for (int i = 0; i < torchCount; i++)
        {
            int idx = Mathf.RoundToInt(i * step);
            if (idx >= validNodes.Count) idx = validNodes.Count - 1;
            MazeNode node = validNodes[idx];
            PlaceTorchAt(node);
            added++;
            yield return LoadingUtils.GetProgress(added, torchCount);
        }

        yield return 1;
    }

    private IEnumerator<float> ActivateMazeTorches()
    {
        int averageAmount = Mathf.CeilToInt(currentMaze.Torches.Count * CurrentData.DifficultyData.LitTorchRatio);
        int torchCount = 0;
        foreach (Torch torch in currentMaze.Torches)
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

    public void PlaceTorchAt(MazeNode node, bool preset = false, Cardinal direction = Cardinal.North)
    {
        Torch torch = mazeManager.GetMazeTorch(CurrentData);
        torch.transform.SetParent(torchContainer);
        currentMaze.AddTorch(node, torch, preset, direction);
    }
    #endregion

    #region Pooling
    private MazeWall GetWall(string id)
    {
        MazeWall wall;
        if (walls.ContainsKey(id))
        {
            wall = walls[id];
        }
        else
        {
            wall = Instantiate(wallPrefab, wallsContainer);
            wall.OnWallCreated(signalBus);
            wall.name = $"Wall {id}";
            walls.Add(id, wall);
        }
        wall.Activate();
        return wall;
    }

    private MazeNode GetNode(Coordinate coordinate)
    {
        if (instantiatedNodes.ContainsKey(coordinate))
        {
            return instantiatedNodes[coordinate];
        }

        MazeNode newNode = Instantiate(nodePrefab, nodeContainer);
        newNode.DebugColor(Color.grey);
        newNode.name = $"Node {coordinate.X},{coordinate.Y}";
        newNode.transform.localPosition = new Vector2(coordinate.X, coordinate.Y);
        instantiatedNodes.Add(coordinate, newNode);
        return newNode;
    }

    public Gate GetGate()
    {
        foreach (Gate gate in gates)
        {
            if (!gate.IsActive) return gate;
        }

        Gate newGate;
        newGate = Instantiate(gatePrefab, itemsContainer);
        newGate.OnWallCreated(signalBus);

        newGate.Activate();
        gates.Add(newGate);
        return newGate;
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
            if (distance >= minDistance)
            {
                return node;
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
        int furthestNodeAvailableDistance = 0;
        MazeNode furthestNode = null;

        int totalDistance;
        int iterations;

        foreach (MazeNode node in nodes)
        {
            totalDistance = 0;
            iterations = 0;

            foreach (MazeNode focusNode in focusNodes)
            {
                iterations++;
                int distance = MazeUtils.GetManhatthanDistanceFromNodeToNode(focusNode, node);
                totalDistance += distance;
            }

            totalDistance = totalDistance / iterations;

            if (totalDistance < minDistance) continue;

            if (totalDistance >= minDistance) return node;
            
            if(totalDistance > furthestNodeAvailableDistance)
            {
                furthestNode = node;
                furthestNodeAvailableDistance = totalDistance;
            }
        }

        currentMaze.MarkNodeAsUsed(furthestNode);
        return furthestNode;
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

    private MazeNode GetAvailableNode(bool forWall = false)
    {
        List<MazeNode> freeNodes = new List<MazeNode>(currentMaze.FreeNodes);
        freeNodes.Shuffle();
        MazeNode node = null;
        if (forWall)
        {
            while (freeNodes.Count > 0)
            {
                node = freeNodes.GetRandom();
                freeNodes.Remove(node);

                if (node.HasAnyWall())
                {
                    currentMaze.MarkNodeAsUsed(node);
                    return node;
                }
            }
        }
        else
        {
            node = freeNodes.GetRandom();
        }
        currentMaze.MarkNodeAsUsed(node);
        return node;
    }
    #endregion

}

public struct ItemPositionData
{
    public Item Item;
    public PresetItemData PresetData;
    public ItemPositionData(Item item, PresetItemData presetData)
    {
        Item = item;
        PresetData = presetData;
    }
}
