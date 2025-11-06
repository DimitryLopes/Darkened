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

        IEnumerator<float> step5enumerator = AddItems();
        LoadingStep step5 = new LoadingStep(step5enumerator, coroutiner, "Forcing your build");

        IEnumerator<float> step6enumerator = AddGates();
        LoadingStep step6 = new LoadingStep(step6enumerator, coroutiner, "Open sezame");

        IEnumerator<float> step7enumerator = AddTorches();
        LoadingStep step7 = new LoadingStep(step7enumerator, coroutiner, "adicionando lá iluminación");

        List<LoadingStep> steps = new List<LoadingStep>
        {
            step1, step2, step3, step5, step6, step7
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

        if (node.IsOnEdge(direction)) 
            wall.SetAsWall(borderWallsTilemap);
        else 
            wall.SetAsWall(wallTilemap);
    }
    #endregion

    #region Step 3

    #endregion

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
    #endregion
    private void ClearMaze()
    {
        wallTilemap.ClearAllTiles();
        borderWallsTilemap.ClearAllTiles();
        nodeTilemap.ClearAllTiles();
        generationPath.Clear();
    }
}
