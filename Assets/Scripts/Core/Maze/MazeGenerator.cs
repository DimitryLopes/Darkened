
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    //wall database is directly injected because walls are not created by a factory
    [Inject]
    private WallDatabase wallDatabase;

    [SerializeField]
    private Node nodePrefab;

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

    private Dictionary<Coordinate, Node> instantiatedNodes = new();
    private List<Node> generationPath = new List<Node>();
    private List<Node> generationNeighbors = new List<Node>();
    private Dictionary<string, MazeWall> walls = new();
    private List<Gate> gates = new List<Gate>();

    private Maze currentMaze;

    private LevelData CurrentData => currentMaze.Data;

    private void ClearMaze()
    {
        if(currentMaze != null)
        {
            foreach (Node node in currentMaze.Nodes)
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
        screenManager.Show<UILoadingScreen>(controller);

        signalBus.Fire(new OnMazeLoadStartedSignal(currentMaze));
    }

    public void CreateMaze(PresetLevelData preset, MazeManager mazeManager)
    {
        this.mazeManager = mazeManager;
        ClearMaze();
        currentMaze = new Maze(preset);

        int width = preset.SizeData.Width;
        int height = preset.SizeData.Height;
        Node[,] nodes = new Node[width, height];

        LoadingOperation mazeLoadingOperation = MazeLoadOperationPreset(preset, nodes);
        LoadingScreenController controller = new LoadingScreenController(mazeLoadingOperation, OnMazeGenerationFinish, audioManager);
        screenManager.Show<UILoadingScreen>(controller);

        signalBus.Fire(new OnMazeLoadStartedSignal(currentMaze));
    }

    private LoadingOperation MazeLoadOperation()
    {
        IEnumerator<float> step1enumerator = CreateBase();
        LoadingStep step1 = new LoadingStep(step1enumerator, coroutiner, "Sweeping the floor, badly");

        IEnumerator<float> step2enumerator = CreateWalls();
        LoadingStep step2 = new LoadingStep(step2enumerator, coroutiner, "Making the duck proud");

        IEnumerator<float> step3enumerator = CreatePath();
        LoadingStep step3 = new LoadingStep(step3enumerator, coroutiner, "Sneaking in");

        //IEnumerator<float> step4enumerator = RemoveDeadEnds();
        //LoadingStep step4 = new LoadingStep(step4enumerator, coroutiner, "Removing dead ends");

        IEnumerator<float> step5enumerator = AddItems();
        LoadingStep step5 = new LoadingStep(step5enumerator, coroutiner, "Forcing your build");

        IEnumerator<float> step6enumerator = AddGates();
        LoadingStep step6 = new LoadingStep(step6enumerator, coroutiner, "Open sezame");

        IEnumerator<float> step7enumerator = AddTorches();
        LoadingStep step7 = new LoadingStep(step7enumerator, coroutiner, "adicionando lá iluminación");

        List<LoadingStep> steps = new List<LoadingStep>
        {
            step1, step2, step3, /*step4,*/ step5, step6, step7
        };

        LoadingOperation operation = new LoadingOperation(steps, coroutiner);
        return operation;
    }

    private LoadingOperation MazeLoadOperationPreset(PresetLevelData preset, Node[,] nodes)
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
        Node[,] nodes = new Node[CurrentData.Width, CurrentData.Height];
        Node node;
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
        Node currentNode;
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

    private Stack<Node> GetNodeStack()
    {
        Stack<Node> stack = new Stack<Node>();
        Coordinate coordinate = new Coordinate(mazeManager.CurrentStartingNode.X, mazeManager.CurrentStartingNode.Y);
        stack.Push(currentMaze.NodesByCoordinate[coordinate]);

        return stack;
    }

    private void SetSpawnPoints(Node[,] nodes)
    {
        Node startNode = SetStartingPoint(nodes, CurrentData);
        mazeManager.CurrentStartingNode = startNode;
        currentMaze.FreeNodes.Remove(startNode);

        Node enemySpawn = GetAvailableNodeAwayFrom(startNode, nodes, CurrentData.Size);
        mazeManager.EnemyStartingNode = enemySpawn;
    }

    private IEnumerator<float> CreatePath()
    {
        float visitedNodes = 0;
        Stack<Node> stack = GetNodeStack();
        bool backtracking = false;
        int targetProgress = currentMaze.Nodes.Length * 2;
        List<Node> neighbors;

        while (stack.Count > 0)
        {
            Node currentNode = stack.Pop();
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

    private void RemoveDeadEndWalls(Node node)
    {
        List<Cardinal> cardinals = EnumUtils.GetEnumValues<Cardinal>();
        if (node.IsOnCorner)
        {
            List<Node> cornerNeighbors = GetNeighbors(node, false);
            foreach (Node corner in cornerNeighbors)
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

            if (!node.IsOnEdge(direction) && node.HasWall(direction))
            {
                Node neighbor = MazeUtils.GetNodeAtCardinalFromNode(direction, node, currentMaze);
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
            PositionItem(items[i]);
            items[i].Activate();
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
        List<Node> shuffledNodes = new(currentMaze.FreeNodes);
        shuffledNodes.Shuffle();

        switch (item.GenerationData.SpawnType)
        {
            case SpawnType.Node:
                Node node = GetAvailableNodeAwayFrom(currentMaze.UsedNodes, shuffledNodes, item.GenerationData.MinDistanceFromThings);
                currentMaze.MarkNodeAsUsed(node, item);

                item.transform.position = node.transform.position;
                item.transform.rotation = node.transform.rotation;
                break;
            case SpawnType.Wall:
                (MazeWall, Cardinal) wallCardinal = GetAvailableWallAwayFrom(currentMaze.UsedNodes, shuffledNodes, item, false);
                PlaceObjectOnWall(item, wallCardinal.Item2, wallCardinal.Item1);
                break;
            case SpawnType.EdgeWalls:
                (MazeWall, Cardinal) edgewallCardinal = GetAvailableWallAwayFrom(currentMaze.UsedNodes, currentMaze.EdgeNodes.Values, item, true);
                PlaceObjectOnWall(item, edgewallCardinal.Item2, edgewallCardinal.Item1);
                break;
        }
        item.transform.SetParent(itemsContainer);
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
            Gate gate = GetGate();

            Node availableNode = GetAvailableNode(true, true);
            (MazeWall,Cardinal) wallCardinal = availableNode.GetAnyWall();
            Node neighbor = MazeUtils.GetNodeAtCardinalFromNode(wallCardinal.Item2, availableNode, currentMaze);
            
            neighbor.ReplaceWall(NodeUtils.GetOppositeCardinal(wallCardinal.Item2), gate);
            availableNode.ReplaceWall(wallCardinal.Item2, gate);
            wallCardinal.Item1.Deactivate();

            Switch switchItem = mazeManager.GetAvailableItem(ItemType.Switch) as Switch;
            gate.Setup((availableNode, neighbor), wallCardinal.Item1);
            switchItem.Associate(gate);
            PositionItem(switchItem);
            walls[GetWallKey(availableNode, wallCardinal.Item2)] = gate;

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

    private List<Node> GetNeighbors(Node node, bool excludeVisited, bool removeNulls = true)
    {
        generationNeighbors.Clear();
        MazeUtils.ExecuteActionWithAllCardinals(AddToNeighborsList, node, ref generationNeighbors);

        generationNeighbors.RemoveAll(item => (item == null && removeNulls) || (excludeVisited && item.Visited));

        return generationNeighbors;
    }

    private Node AddToNeighborsList(Cardinal direction, Node node)
    {
        Node neighbor = MazeUtils.GetNodeAtCardinalFromNode(direction, node, currentMaze);
        if (neighbor != null)
        {
            return neighbor;
        }
        return null;
    }

    private Node SetStartingPoint(Node[,] nodes, LevelData data)
    {
        int startingX = Random.Range(0, data.Width);
        currentMaze.MarkNodeAsUsed(nodes[startingX, 0], null);
        return nodes[startingX, 0];
    }

    #region Walls
    public void RemoveWallBetween(Node nodeA, Node nodeB)
    {
        Cardinal direction = NodeUtils.GetCardinalDirection(nodeB, nodeA);
        //This deactivates the wall in nodeA, B and in the wall dictionary
        nodeA.RemoveWall(direction);
    }

    public void ActivateWallBetween(Node nodeA, Node nodeB)
    {
        Cardinal direction = NodeUtils.GetCardinalDirection(nodeB, nodeA);
        //This activates the wall in nodeA, B and in the wall dictionary
        nodeA.ActivateWall(direction);
    }

    public void RemoveWall(Node node, Cardinal direction)
    {
        node.RemoveWall(direction);
    }

    private void AddNodeWall(Cardinal direction, Node node)
    {
        string wallID = GetWallKey(node, direction);
        MazeWall wall = GetWall(wallID);
        MazeWall currentWall = node.GetWall(direction);

        if (currentWall != null)
        {
            if (currentWall != wall)
            {
                node.ReplaceWall(direction, wall);
                return;
            }
            node.ActivateWall(direction);
            return;
        }

        node.AddWall(direction, wall);
        wall.AddNode(node);
        if (!node.IsOnEdge(direction)) return;

        wall.transform.SetParent(edgeWallsContainer);
    }

    private string GetWallKey(Node node, Cardinal direction)
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

    private void SetNodeEdges(Node node, MazeSizeData data)
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
    private IEnumerator<float> CreateNodesPreset(PresetLevelData preset, Node[,] nodes)
    {
        int width = preset.SizeData.Width;
        int height = preset.SizeData.Height;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Node node = GetNode(new Coordinate(x, y));
                nodes[x, y] = node;
                node.Activate();
                node.SetCoordinate(x, y);
                SetNodeEdges(node, preset.SizeData);
                yield return LoadingUtils.GetProgress(y * width + x, width * height);
            }
        }
        currentMaze.SetNodes(nodes);
    }

    private IEnumerator<float> CreateWallsPreset(PresetLevelData preset, Node[,] nodes)
    {
        var wallPositions = preset.WallPositions;
        int totalWalls = wallPositions.Count;
        int currentWall = 0;

        foreach (var wallData in wallPositions)
        {
            Node node = nodes[wallData.Coordinate.X, wallData.Coordinate.Y];
            Node neighbor = MazeUtils.GetNodeAtCardinalFromNode(wallData.Direction, node, currentMaze);
            MazeWall wall;
            switch (wallData.WallType)
            {
                default:
                    string wallID = GetWallKey(node, wallData.Direction);
                    wall = GetWall(wallID);
                    break;
                case WallType.gate:
                    wall = GetGate();

                    (wall as Gate).Setup((node, neighbor));
                    break;
            }
            
            node.AddWall(wallData.Direction, wall);
            if (neighbor)
            {
                neighbor.AddWall(NodeUtils.GetOppositeCardinal(wallData.Direction), wall);
            }

            if (node.IsOnEdge(wallData.Direction))
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

    private IEnumerator<float> CreateItemsPreset(PresetLevelData preset, Node[,] nodes)
    {
        var presetItemsData = new List<PresetItemData>(preset.Items);
        int totalItems = presetItemsData.Count;
        int currentItem = 0;

        List<ItemPositionData> items = mazeManager.GetPresetMissionItems(preset);

        foreach (ItemPositionData itemPositionData in items)
        {
            switch (itemPositionData.Item.GenerationData.SpawnType)
            {
                case SpawnType.Node:
                    Node node = nodes[itemPositionData.PresetData.Coordinate.X, itemPositionData.PresetData.Coordinate.Y];
                    itemPositionData.Item.transform.position = node.transform.position;
                    break;
                case SpawnType.Wall:
                case SpawnType.EdgeWalls:
                    Node edgeNode = nodes[itemPositionData.PresetData.Coordinate.X, itemPositionData.PresetData.Coordinate.Y];
                    MazeWall wall = NodeUtils.GetWallAt(edgeNode, itemPositionData.PresetData.Direction, currentMaze);
                    itemPositionData.Item.transform.position = wall.transform.position;
                    itemPositionData.Item.transform.rotation = wall.transform.rotation;
                    break;
            }
            presetItemsData.Remove(itemPositionData.PresetData);
        }

        foreach (var itemData in presetItemsData)
        {
            if (itemData.Item.Type == ItemType.DefaultTorch)
            {
                PlaceTorchAt(nodes[itemData.Coordinate.X, itemData.Coordinate.Y], itemData.Direction);
                currentItem++;
                yield return LoadingUtils.GetProgress(currentItem, totalItems);
                continue;
            }
            Item item = mazeManager.GetAvailableItem(itemData.Item.Type);
            item.transform.SetParent(itemsContainer);

            switch (item.Type)
            {
                case ItemType.PressurePlate:
                    SetupPressurePlatePreset(nodes, itemData, item);
                    break;
                case ItemType.Switch:
                    SetupSwitchPreset(nodes, itemData, item);
                    break;
            }

            switch (item.GenerationData.SpawnType)
            {
                case SpawnType.Node:
                    Node node = nodes[itemData.Coordinate.X, itemData.Coordinate.Y];
                    currentMaze.MarkNodeAsUsed(node, item);
                    item.transform.position = node.transform.position;
                    break;
                case SpawnType.Wall:
                case SpawnType.EdgeWalls:
                    Node edgeNode = nodes[itemData.Coordinate.X, itemData.Coordinate.Y];
                    currentMaze.MarkNodeAsUsed(edgeNode, item);
                    MazeWall wall = NodeUtils.GetWallAt(edgeNode, itemData.Direction, currentMaze);
                    PlaceObjectOnWall(item, itemData.Direction, wall);
                    break;
            }

            item.Activate();
            currentItem++;
            yield return LoadingUtils.GetProgress(currentItem, totalItems);
        }

        void SetupSwitchPreset(Node[,] nodes, PresetItemData itemData, Item item)
        {
            SetupPreset<IToggable>(nodes, itemData, item,
                (itm, target) => ((Switch)itm).Associate(target));
        }

        void SetupPressurePlatePreset(Node[,] nodes, PresetItemData itemData, Item item)
        {
            SetupPreset<ITrap>(nodes, itemData, item,
                (itm, target) => ((PressurePlate)itm).Associate(target));
        }

        void SetupPreset<TInterface>(Node[,] nodes, PresetItemData itemData, Item item, System.Action<Item, TInterface> associateAction) where TInterface : class
        {
            string raw = itemData.AssociateWith;
            string[] parts = raw.Split('&');

            foreach (string part in parts)
            {
                string link = part.Trim();
                if (string.IsNullOrEmpty(link)) continue;

                if (link.StartsWith("wall", System.StringComparison.OrdinalIgnoreCase))
                {
                    // Format: wall north [8,5]
                    int bracketIndex = link.IndexOf('[');
                    if (bracketIndex == -1) continue;

                    string dirString = link.Substring(5, bracketIndex - 5).Trim();
                    string coordString = link.Substring(bracketIndex).Trim(); // [x,y]

                    if (!System.Enum.TryParse<Cardinal>(dirString, true, out var dir)) continue;
                    if (!TryParseCoordinate(coordString, out Coordinate coord)) continue;

                    Node node = nodes[coord.X, coord.Y];
                    MazeWall wall = node.GetWall(dir);

                    if (wall is TInterface target)
                        associateAction(item, target);
                }
                else
                {
                    // Format: [x,y]
                    if (!TryParseCoordinate(link, out Coordinate coord)) continue;

                    Node node = nodes[coord.X, coord.Y];

                    if (node.UsedBy is TInterface target)
                        associateAction(item, target);
                }
            }
        }
    }

    private bool TryParseCoordinate(string input, out Coordinate result)
    {
        result = default;

        if (!input.StartsWith("[") || !input.EndsWith("]")) return false;

        string[] parts = input.Substring(1, input.Length - 2).Split(',');
        if (parts.Length != 2) return false;

        if (int.TryParse(parts[0], out int x) && int.TryParse(parts[1], out int y))
        {
            result = new Coordinate(x, y);
            return true;
        }

        return false;
    }

    private IEnumerator<float> SetPositionsPreset(PresetLevelData preset, Node[,] nodes)
    {
        Node startNode = nodes[preset.StartPosition.X, preset.StartPosition.Y];
        mazeManager.CurrentStartingNode = startNode;

        Node enemySpawnNode = nodes[preset.EnemySpawnPosition.X, preset.EnemySpawnPosition.Y];
        mazeManager.EnemyStartingNode = enemySpawnNode;
        mazeManager.SetupMissions();
        yield return 1;
    }
    #endregion

    public void PlaceObjectOnWall(Item item, Cardinal direction, MazeWall wall)
    {
        float rotation = NodeUtils.GetWallRotationByCardinal(direction);
        item.transform.SetPositionAndRotation(wall.transform.position, Quaternion.Euler(0, 0, rotation));
    }

    #region Torches
    //THIS ONLY WORKS WITH RANDOM GENERATION
    private IEnumerator<float> AddTorches()
    {
        int minTorchCount = (CurrentData as RandomLevelData).MinTorchCount;
        int maxTorchCount = (CurrentData as RandomLevelData).MaxTorchCount;
        int torchCount = Random.Range(minTorchCount, maxTorchCount + 1);

        // Filtra nós válidos (sem item/tocha)
        List<Node> validNodes = new List<Node>(currentMaze.FreeNodes);

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
            Node node = validNodes[idx];
            (MazeWall, Cardinal) wallToPlaceTorchOn = node.GetAnyDefaultWall();
            PlaceTorchAt(node, wallToPlaceTorchOn.Item2);
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

    public void PlaceTorchAt(Node node, Cardinal direction)
    {
        Torch torch = mazeManager.GetMazeTorch(CurrentData);
        torch.transform.SetParent(torchContainer);
        MazeWall wall = NodeUtils.GetWallAt(node, direction, currentMaze);
        PlaceObjectOnWall(torch, direction, wall);
        currentMaze.AddTorch(node, torch, direction);
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
            wall = Instantiate(wallDatabase.Walls[WallType.wall], wallsContainer);
            wall.OnWallCreated(signalBus);
            wall.name = $"Wall {id}";
            walls.Add(id, wall);
        }
        wall.Activate();
        return wall;
    }

    private Node GetNode(Coordinate coordinate)
    {
        if (instantiatedNodes.ContainsKey(coordinate))
        {
            return instantiatedNodes[coordinate];
        }

        Node newNode = Instantiate(nodePrefab, nodeContainer);
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
            if (!gate.IsActive)
            {
                gate.Activate();
                return gate;
            } 
        }

        Gate newGate;
        newGate = Instantiate(wallDatabase.Walls[WallType.gate] as Gate, itemsContainer);
        newGate.OnWallCreated(signalBus);

        newGate.Activate();
        gates.Add(newGate);
        return newGate;
    }
    #endregion

    #region Utils

    private Node GetAvailableNodeAwayFrom(Node focusNode, IEnumerable nodes, int minDistance)
    {
        int maxDistance = 0;
        Node fallbackNode = null;

        foreach (Node node in nodes)
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

    private Node GetAvailableNodeAwayFrom(IEnumerable focusNodes, IEnumerable nodes, int minDistance)
    {

        if(focusNodes == null || !focusNodes.Cast<Node>().Any())
        {
            return nodes.Cast<Node>().First();
        }

        int furthestNodeAvailableDistance = 0;
        Node furthestNode = null;

        int totalDistance;
        int iterations;


        foreach (Node node in nodes)
        {
            totalDistance = 0;
            iterations = 0;

            foreach (Node focusNode in focusNodes)
            {
                iterations++;
                int distance = MazeUtils.GetManhatthanDistanceFromNodeToNode(focusNode, node);
                totalDistance += distance;
            }

            totalDistance = totalDistance / iterations;

            if (totalDistance < minDistance) continue;

            if (totalDistance >= minDistance) return node;

            if (totalDistance > furthestNodeAvailableDistance)
            {
                furthestNode = node;
                furthestNodeAvailableDistance = totalDistance;
            }
        }
        return furthestNode;
    }

    private (MazeWall, Cardinal) GetAvailableWallAwayFrom(IEnumerable focusNodes, IEnumerable nodes, Item item, bool edge = false)
    {
        (MazeWall, Cardinal) wallCardinal;
        Node availableNode = GetAvailableNodeAwayFrom(focusNodes, nodes, item.GenerationData.MinDistanceFromThings);
        if (edge)
        {
            wallCardinal = availableNode.GetRandomWallOnEdge();
        }
        else
        {
            wallCardinal = availableNode.GetAnyDefaultWall();
        }
        
        currentMaze.MarkNodeAsUsed(availableNode, item);
        return wallCardinal;
    }

    private Node GetAvailableNode(bool forWall = false, bool excludeBorders = false)
    {
        List<Node> freeNodes = new List<Node>(currentMaze.FreeNodes);
        if (excludeBorders)
        {
            var borderSet = new HashSet<Node>(currentMaze.EdgeNodes.Values);
            freeNodes.RemoveAll(n => borderSet.Contains(n));
        }

        freeNodes.Shuffle();
        Node node = null;
        if (forWall)
        {
            while (freeNodes.Count > 0)
            {
                node = freeNodes.GetRandom();
                freeNodes.Remove(node);

                if (node.HasAnyWall())
                {
                    return node;
                }
            }
        }
        else
        {
            node = freeNodes.GetRandom();
        }
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
