using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;

public class MazeGeneration : MonoBehaviour
{
    [Inject]
    private Coroutiner coroutiner;
    [Inject]
    private SignalBus signalBus;
    [Inject]
    private ScreenManager screenManager;
    [Inject]
    private AudioManager audioManager;

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
    private List<Node> generationNeighbors = new List<Node>();
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

        //IEnumerator<float> step5enumerator = AddItems();
        //LoadingStep step5 = new LoadingStep(step5enumerator, coroutiner, "Forcing your build");

        //IEnumerator<float> step6enumerator = AddGates();
        //LoadingStep step6 = new LoadingStep(step6enumerator, coroutiner, "Open sezame");

        IEnumerator<float> step7enumerator = AddTorches();
        LoadingStep step7 = new LoadingStep(step7enumerator, coroutiner, "adicionando lá iluminación");

        List<LoadingStep> steps = new List<LoadingStep>
        {
            step1, step2, step3, /*step5, step6,*/ step7
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
                node = GetNode(new Coordinate(x,y));
                nodes[x, y] = node;

                PaintNodeTiles(node, x, y);

                if ((y * CurrentData.Height + x) % 10 == 0)
                    yield return LoadingUtils.GetProgress(y * CurrentData.Height + x, CurrentData.Size);
            }
        }

        SetSpawnPoints(nodes);
        currentMaze.SetNodes(nodes);
    }

    private void PaintNodeTiles(Node node, int nodeX, int nodeY)
    {
        int startX = nodeX * 3 + nodeX + 1;
        int startY = nodeY * 3 + nodeY + 1;

        for(int x = 0; x < 3; x++)
        {
            for(int y = 0; y < 3; y++)
            {
                nodeTilemap.SetTile(new Vector3Int(startX + x, startY + y, 0), AssetService.GetFloorTile());
            }
        }
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

        bool isOnEdge = node.IsOnEdge(direction)
        if (isOnEdge) 
            wall.SetAsWall(borderWallsTilemap);
        else 
            wall.SetAsWall(wallTilemap);
        //TODO: Get tiles
        wall.Setup(, isOnEdge);
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
            item.transform.position = wall.transform.position;
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
        item.transform.SetPositionAndRotation(wall.transform.position, Quaternion.Euler(0, 0, rotation));
    }

    #endregion
    public void RemoveWallBetween(Node nodeA, Node nodeB)
    {
        Cardinal direction = NodeUtils.GetCardinalDirection(nodeB, nodeA);
        string wallID = GetWallKey(nodeA, direction);
        MazeWall wall = GetWall(wallID);
        wall.SetAsEmpty(wallTilemap);
    }

    private void RemoveDeadEndWalls(Node node)
    {
        List<Cardinal> cardinals = EnumUtils.GetEnumValues<Cardinal>();
        if (node.IsOnCorner)
        {
            List<Node> cornerNeighbors = GetNodeNeighbors(node, false);
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
        int x = node.Coordinates.X;
        int y = node.Coordinates.Y;

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
    private void ClearMaze()
    {
        wallTilemap.ClearAllTiles();
        borderWallsTilemap.ClearAllTiles();
        nodeTilemap.ClearAllTiles();
        generationPath.Clear();
    }
}
