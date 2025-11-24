using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
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
    [Inject]
    private ItemDataBase itemDataBase;

    [SerializeField, Header("Containers")]
    private Tilemap nodeTilemap;
    [SerializeField]
    private Tilemap wallTilemap;
    [SerializeField]
    private Tilemap borderWallsTilemap;
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

    private Dictionary<Coordinate, Node> createdNodes = new();
    private List<Node> generationPath = new List<Node>();
    private Dictionary<string, MazeWall> walls = new();

    private Maze currentMaze;
    private LevelData CurrentData => currentMaze.Data;

    #region random generation
    public void CreateMaze(RandomLevelData data, MazeManager mazeManager)
    {
        this.mazeManager ??= mazeManager;
        ClearMaze();
        currentMaze = new Maze(data);

        LoadingOperation mazeLoadingOperation = MazeLoadOperation();
        LoadingScreenController controller = new LoadingScreenController(mazeLoadingOperation, OnMazeGenerationFinish, audioManager);
        screenManager.Show<UILoadingScreen>(controller);

        signalBus.Fire(new OnMazeLoadStartedSignal(currentMaze));
    }

    private LoadingOperation MazeLoadOperation()
    {
        IEnumerator<float> step1enumerator = CreateBase();
        LoadingStep step1 = new LoadingStep(step1enumerator, coroutiner, "Sweeping the floor");

        IEnumerator<float> step2enumerator = CreateWalls();
        LoadingStep step2 = new LoadingStep(step2enumerator, coroutiner, "Making the duck proud");

        IEnumerator<float> step3enumerator = CreatePath();
        LoadingStep step3 = new LoadingStep(step3enumerator, coroutiner, "Sneaking in");

        IEnumerator<float> step4enumerator = FillTheGaps();
        LoadingStep step4 = new LoadingStep(step4enumerator, coroutiner, "Filling the gaps");

        IEnumerator<float> step5enumerator = AddItems();
        LoadingStep step5 = new LoadingStep(step5enumerator, coroutiner, "Forcing your build");

        //IEnumerator<float> step6enumerator = AddGates();
        //LoadingStep step6 = new LoadingStep(step6enumerator, coroutiner, "Open sezame");

        IEnumerator<float> step7enumerator = AddTorches();
        LoadingStep step7 = new LoadingStep(step7enumerator, coroutiner, "adicionando lá iluminación");

        List<LoadingStep> steps = new List<LoadingStep>
        {
            step1, step2, step3, step4, step5, /*step6,*/ step7
        };

        LoadingOperation operation = new LoadingOperation(steps, coroutiner);
        return operation;
    }

    #region Step 1
    private IEnumerator<float> CreateBase()
    {
        Node[,] nodes = new Node[CurrentData.Width, CurrentData.Height];
        Node node;

        for (int y = 0; y < CurrentData.Height; y++)
        {
            for (int x = 0; x < CurrentData.Width; x++)
            {
                node = GetNode(new Coordinate(x, y));
                nodes[x, y] = node;

                AssociatedTileToNode(node, x, y);

                if ((y * CurrentData.Height + x) % 10 == 0)
                    yield return LoadingUtils.GetProgress(y * CurrentData.Height + x, CurrentData.Size);
            }
        }

        SetSpawnPoints(nodes);
        currentMaze.SetNodes(nodes);
    }

    private void AssociatedTileToNode(Node node, int nodeX, int nodeY)
    {
        int startX = nodeX * 3 + nodeX + 1;
        int startY = nodeY * 3 + nodeY + 1;
        List<TileData> tiles = new();
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                Vector3Int tilePosition = new Vector3Int(startX + x, startY + y, 0);
                tiles.Add(new TileData(nodeTilemap.GetTile(tilePosition), tilePosition));
                nodeTilemap.SetTile(tilePosition, AssetService.GetFloorTile());
            }
        }
        node.AssociateTiles(tiles);
    }

    private void SetSpawnPoints(Node[,] nodes)
    {
        Node startNode = SetStartingPoint(nodes, CurrentData);
        mazeManager.CurrentStartingNode = startNode;
        currentMaze.FreeNodes.Remove(startNode);

        Node enemySpawn = GetAvailableNodeAwayFrom(startNode, nodes, CurrentData.Size);
        mazeManager.EnemyStartingNode = enemySpawn;
    }

    private Node SetStartingPoint(Node[,] nodes, LevelData data)
    {
        int startingX = UnityEngine.Random.Range(0, data.Width);
        currentMaze.MarkNodeAsUsed(nodes[startingX, 0], null);
        return nodes[startingX, 0];
    }
    #endregion

    #region Step 2
    private IEnumerator<float> CreateWalls()
    {
        int length = currentMaze.Nodes.Length;
        Node currentNode;

        for (int y = 0; y < CurrentData.Height; y++)
        {
            for (int x = 0; x < CurrentData.Width; x++)
            {
                currentNode = currentMaze.Nodes[x, y];
                MazeUtils.ExecuteActionWithAllCardinals(AddNodeWall, currentNode);
            }

            yield return LoadingUtils.GetProgress(y * CurrentData.Height, length);
        }
    }

    private void AddNodeWall(Cardinal direction, Node node)
    {
        string wallID = GetWallKey(node, direction);
        MazeWall wall = GetWall(wallID);

        if (!node.HasWall(direction))
        {
            node.AddWall(direction, wall);
            wall.AddNode(node);
        }

        bool isOnBorder = node.IsOnBorder(direction);

        List<TileData> associatedTiles = new List<TileData>();
        associatedTiles = GetWallAssociatedTiles(node, direction);
        wall.Setup(associatedTiles, isOnBorder);

        if (isOnBorder)
            wall.SetAsWall(borderWallsTilemap);
        else
            wall.SetAsWall(wallTilemap);

    }

    private List<TileData> GetWallAssociatedTiles(Node node, Cardinal direction)
    {
        int startX = node.X * 3 + node.X + 1;
        int startY = node.Y * 3 + node.Y + 1;

        switch (direction)
        {
            case Cardinal.North:
                startY += 3;
                break;
            case Cardinal.East:
                startX += 3;
                break;
            case Cardinal.South:
                startY -= 1;
                break;
            case Cardinal.West:
                startX -= 1;
                break;
        }

        List<TileData> tiles = new();
        for (int i = 0; i < 3; i++)
        {
            Vector3Int tilePosition;
            switch (direction)
            {
                case Cardinal.North:
                case Cardinal.South:
                    tilePosition = new Vector3Int(startX + i, startY, 0);
                    break;
                case Cardinal.East:
                case Cardinal.West:
                    tilePosition = new Vector3Int(startX, startY + i, 0);
                    break;
                default:
                    tilePosition = new Vector3Int(startX, startY, 0);
                    break;
            }
            tiles.Add(new TileData(nodeTilemap.GetTile(tilePosition), tilePosition));
        }
        return tiles;
    }
    #endregion

    #region Step 3
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
            neighbors = GetNodeNeighbors(currentNode, true);
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
    #endregion

    #region Step 4
    private IEnumerator<float> FillTheGaps()
    {
        int length = currentMaze.Nodes.Length;
        for (int x = 0; x <= currentMaze.Data.Width; x++)
        {
            for (int y = 0; y <= currentMaze.Data.Height; y++)
            {
                Vector3Int pos = new Vector3Int(x * 4, y * 4, 0);
                if (x == 0 || x >= currentMaze.Data.Width || y == 0 || y >= currentMaze.Data.Height)
                {
                    borderWallsTilemap.SetTile(pos, AssetService.GetWallTile());
                    wallTilemap.SetTile(pos, AssetService.GetWallTile());
                    continue;
                }
                bool isBetween = CheckIfTileIsBetweenWalls(currentMaze.Nodes[x, y], currentMaze.Nodes[x - 1, y - 1]);

                if (isBetween)
                {
                    wallTilemap.SetTile(pos, AssetService.GetWallTile());
                }
                else
                {
                    wallTilemap.SetTile(pos, AssetService.GetFloorTile());
                }
                yield return LoadingUtils.GetProgress(y * CurrentData.Height, length);
            }
        }
        yield return LoadingUtils.GetProgress(length, length);
    }

    private bool CheckIfTileIsBetweenWalls(Node topRightNode, Node bottomLeftNode)
    { bool south = topRightNode.GetWall(Cardinal.South).IsActive;
        bool north = bottomLeftNode.GetWall(Cardinal.North).IsActive;
        bool east = bottomLeftNode.GetWall(Cardinal.East).IsActive;
        bool west = topRightNode.GetWall(Cardinal.West).IsActive;
        int wallCount = (south ? 1 : 0) + (north ? 1 : 0) + (east ? 1 : 0) + (west ? 1 : 0);

        return wallCount >= 2;
    }
    #endregion

    #region Step 5
    private IEnumerator<float> AddItems()
    {
        List<Item> items = mazeManager.GetMissionItems();
        int randomItemAmount = GetRandomItemAmount(CurrentData.SizeData.AverageAdditionalItemAmount);
        //        
        List<(float, ItemType)> weightedItems = new List<(float, ItemType)>();
        float totalValue = 0f;
        foreach (var data in itemDataBase.DataBase)
        {
            totalValue += data.Probability;
            weightedItems.Add((totalValue, data.Type));
        }

        float randomValue = UnityEngine.Random.Range(0, 100f);

        for (int i = 0; i < randomItemAmount; i++)
        {
            //they are being added from lowest to highest, meaning that the first one that matches is the selected one
            foreach (var item in weightedItems)
            {
                if (randomValue <= item.Item1)
                {
                    items.Add(mazeManager.GetAvailableItem(item.Item2));
                    break;
                }
            }
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
        int itemAmount = Mathf.RoundToInt(itemAverage + randomValue);

        return itemAmount;
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

                item.transform.position = node.Position;
                //Set rotation here
                break;
            case SpawnType.Wall:
                (MazeWall, Cardinal) wallCardinal = (null, Cardinal.North);
                while (wallCardinal.Item1 != null)
                {
                    Node wallNode = GetAvailableNodeAwayFrom(currentMaze.UsedNodes, shuffledNodes, item.GenerationData.MinDistanceFromThings);
                    wallCardinal = wallNode.GetAnyDefaultWall();
                }
                PlaceItemOnWall(item, wallCardinal.Item2, wallCardinal.Item1);
                break;
            case SpawnType.EdgeWalls:
                (MazeWall, Cardinal) edgeWallCardinal = (null, Cardinal.North);
                while (edgeWallCardinal.Item1 != null)
                {
                    Node wallNode = GetAvailableNodeAwayFrom(currentMaze.UsedNodes, currentMaze.EdgeNodes.Values, item.GenerationData.MinDistanceFromThings);
                    wallCardinal = wallNode.GetAnyDefaultWall();
                }
                PlaceItemOnWall(item, edgeWallCardinal.Item2, edgeWallCardinal.Item1);
                break;
        }
        item.transform.SetParent(itemsContainer);
    }
    #endregion

    #region Step 6 
    private IEnumerator<float> AddGates()
    {
        int gateCount = Mathf.FloorToInt(currentMaze.Data.SizeData.AverageGateAmount);
        float additionalGateChance = currentMaze.Data.SizeData.AverageGateAmount - gateCount;
        float random = UnityEngine.Random.Range(0, 1f);
        if (random < additionalGateChance)
        {
            gateCount++;
        }

        for (int i = 0; i < gateCount; i++)
        {
            List<MazeWall> availableWalls = GetAvailableGatePositions();
            //Find walls that can be used for Switches 
            yield return LoadingUtils.GetProgress(i + 1, gateCount);
        }


        signalBus.Fire(new OnItemsLoadFinishSignal());
        shadowCreator.Create(mazeCollider);
        yield return 1;
    }

    private List<MazeWall> GetAvailableGatePositions()
    {
        List<MazeWall> availableGates = new List<MazeWall>();

        Node northNeighbor;
        Node southNeighbor;
        Node eastNeighbor;
        Node westNeighbor;

        foreach (var node in currentMaze.Nodes)
        {
            var north = MazeUtils.GetNodeAtCardinalFromNode(Cardinal.North, node, currentMaze);
            var south = MazeUtils.GetNodeAtCardinalFromNode(Cardinal.South, node, currentMaze);
            var east = MazeUtils.GetNodeAtCardinalFromNode(Cardinal.East, node, currentMaze);
            var west = MazeUtils.GetNodeAtCardinalFromNode(Cardinal.West, node, currentMaze);

            CheckOne(north, Cardinal.East, Cardinal.South);
            CheckOne(north, Cardinal.West, Cardinal.South);

            CheckOne(south, Cardinal.East, Cardinal.North);
            CheckOne(south, Cardinal.West, Cardinal.North);

            CheckOne(east, Cardinal.South, Cardinal.West);
            CheckOne(east, Cardinal.North, Cardinal.West);

            CheckOne(west, Cardinal.South, Cardinal.East);
            CheckOne(west, Cardinal.North, Cardinal.East);

            CheckTwo(west, east, Cardinal.North);
            CheckTwo(west, east, Cardinal.South);

            CheckTwo(north, south, Cardinal.East);
            CheckTwo(north, south, Cardinal.West);

            void CheckOne(Node neighbor, Cardinal dir, Cardinal complement)
            {
                if (neighbor == null) return;
                if (neighbor.HasWall(dir) && node.HasWall(dir) && node.HasWall(complement))
                    availableGates.Add(node.GetWall(dir));
            }

            void CheckTwo(Node nodeA, Node nodeB, Cardinal dir)
            {
                if (nodeA == null || nodeB == null) return;
                if (nodeA.HasWall(dir) && nodeB.HasWall(dir) && node.HasWall(dir))
                    availableGates.Add(node.GetWall(dir));
            }
        }
        return availableGates;
    }
    #endregion

    #region Step 7
    private IEnumerator<float> AddTorches()
    {
        int minTorchCount = (CurrentData as RandomLevelData).MinTorchCount;
        int maxTorchCount = (CurrentData as RandomLevelData).MaxTorchCount;
        int torchCount = UnityEngine.Random.Range(minTorchCount, maxTorchCount + 1);

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
    public void PlaceTorchAt(Node node, Cardinal direction)
    {
        Torch torch = mazeManager.GetMazeTorch(CurrentData);
        torch.transform.SetParent(torchContainer);
        MazeWall wall = NodeUtils.GetWallAt(node, direction, currentMaze);
        PlaceItemOnWall(torch, direction, wall);
        currentMaze.AddTorch(node, torch, direction);
    }
    #endregion

    public void PlaceItemOnWall(Item item, Cardinal direction, MazeWall wall)
    {
        if (item.Type == ItemType.DefaultTorch)
        {
            item.transform.position = wallTilemap.CellToWorld(wall.AssociatedTiles[1].Position);
            if (direction == Cardinal.North)
            {
                item.transform.localScale = new Vector3(item.transform.localScale.x, -item.transform.localScale.y, item.transform.localScale.z);
                item.transform.position += new Vector3(0, -Constants.Items.WALL_ITEM_OFFSET, 0);
            }
            if (direction == Cardinal.East)
            {
                item.transform.localScale = new Vector3(-item.transform.localScale.x, item.transform.localScale.y, item.transform.localScale.z);
                item.transform.position += new Vector3(-Constants.Items.WALL_ITEM_OFFSET, 0, 0);
            }
            if (direction == Cardinal.West)
            {
                item.transform.position += new Vector3(Constants.Items.WALL_ITEM_OFFSET, 0, 0);
            }
            if (direction == Cardinal.South)
            {
                item.transform.position += new Vector3(0, Constants.Items.WALL_ITEM_OFFSET, 0);
            }
            return;
        }
        float rotation = NodeUtils.GetWallRotationByCardinal(direction);
        item.transform.rotation = Quaternion.Euler(0, 0, rotation);
    }

    #endregion
    public void RemoveWallBetween(Node nodeA, Node nodeB)
    {
        Cardinal direction = NodeUtils.GetCardinalDirection(nodeB, nodeA);
        string wallID = GetWallKey(nodeA, direction);
        MazeWall wall = GetWall(wallID);
        wall.SetAsEmpty(wallTilemap);
        foreach(TileData tile in wall.AssociatedTiles)
        {
            nodeTilemap.SetTile(tile.Position, AssetService.GetFloorTile());
        }
    }

    public void ActivateWallBetween(Node nodeA, Node nodeB)
    {
        Cardinal direction = NodeUtils.GetCardinalDirection(nodeB, nodeA);
        string wallID = GetWallKey(nodeA, direction);
        MazeWall wall = GetWall(wallID);
        wall.SetAsWall(wallTilemap);
    }

    private void RemoveDeadEndWalls(Node node)
    {
        List<Cardinal> cardinals = EnumUtils.GetEnumValues<Cardinal>();
        if (node.IsOnEdge())
        {
            List<Node> BorderNeighbors = GetNodeNeighbors(node, false);
            foreach (Node border in BorderNeighbors)
            {
                RemoveWallBetween(border, node);
            }
            return;
        }

        int wallsRemoved = 0;
        cardinals.Shuffle();
        while (cardinals.Count > 0 && wallsRemoved < 2)
        {
            var direction = cardinals[0];
            cardinals.RemoveAt(0);

            if (!node.IsOnBorder(direction) && node.HasWall(direction))
            {
                Node neighbor = MazeUtils.GetNodeAtCardinalFromNode(direction, node, currentMaze);
                RemoveWallBetween(node, neighbor);
                wallsRemoved++;
            }
        }
    }

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
            else if (distance > maxDistance)
            {
                fallbackNode = node;
                maxDistance = distance;
            }
        }

        return fallbackNode;
    }

    private Node GetAvailableNodeAwayFrom(List<Node> focusNodes, IEnumerable nodes, int minDistance)
    {
        int currentNodeDistance;
        (int,Node) furthestNodeFallbackDistance = (int.MinValue, null);
        foreach (Node node in currentMaze.FreeNodes)
        {
            bool meetsDistance = true;
            currentNodeDistance = 0;
            foreach (Node focusNode in focusNodes)
            {
                int distance = MazeUtils.GetManhatthanDistanceFromNodeToNode(focusNode, node);
                currentNodeDistance += distance;
                if (distance < minDistance)
                {
                    continue;
                }
                else
                {
                    meetsDistance = false;
                }
            }

            if (meetsDistance)
            {
                return node;
            }

            if(currentNodeDistance >= furthestNodeFallbackDistance.Item1)
            {
                furthestNodeFallbackDistance = (currentNodeDistance, node);
            }
        }

        return furthestNodeFallbackDistance.Item2;
    }

    private Node GetNode(Coordinate coordinate)
    {
        if (createdNodes.ContainsKey(coordinate))
        {
            createdNodes[coordinate].Setup(coordinate, CurrentData.SizeData);
            return createdNodes[coordinate];
        }

        Node newNode = new Node(coordinate, CurrentData.SizeData);
        createdNodes.Add(coordinate, newNode);
        return newNode;
    }

    private string GetWallKey(Node node, Cardinal direction)
    {
        int x = node.Coordinate.X;
        int y = node.Coordinate.Y;

        int nx, ny;
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

        if (x > nx || (x == nx && y > ny))
        {
            return string.Format(Constants.Generation.WALL_ID_FORMAT, nx, ny, x, y);
        }
        else
        {
            return string.Format(Constants.Generation.WALL_ID_FORMAT, x, y, nx, ny);
        }
    }
    private MazeWall GetWall(string id)
    {
        MazeWall wall;
        if (walls.ContainsKey(id))
        {
            wall = walls[id];
        }
        else
        {
            wall = new MazeWall();
            walls.Add(id, wall);
        }

        return wall;
    }
    private Stack<Node> GetNodeStack()
    {
        Stack<Node> stack = new Stack<Node>();
        Coordinate coordinate = new Coordinate(mazeManager.CurrentStartingNode.X, mazeManager.CurrentStartingNode.Y);
        stack.Push(currentMaze.NodesByCoordinate[coordinate]);

        return stack;
    }
    private List<Node> GetNodeNeighbors(Node node, bool excludeVisited, bool removeNulls = true)
    {
        List<Node> neighbors = new List<Node>();
        MazeUtils.ExecuteActionWithAllCardinals(AddToNeighborsList, node, ref neighbors);

        neighbors.RemoveAll(item => (item == null && removeNulls) || (excludeVisited && item.Visited));

        Node AddToNeighborsList(Cardinal direction, Node node)
        {
            Node neighbor = MazeUtils.GetNodeAtCardinalFromNode(direction, node, currentMaze);
            if (neighbor != null)
            {
                return neighbor;
            }
            return null;
        }

        return neighbors;
    }

    #endregion
    private void OnMazeGenerationFinish()
    {
        signalBus.Fire(new OnMazeLoadFinishSignal(currentMaze));
    }

    private void ClearMaze()
    {
        wallTilemap.ClearAllTiles();
        borderWallsTilemap.ClearAllTiles();
        nodeTilemap.ClearAllTiles();
        generationPath.Clear();
    }
}
