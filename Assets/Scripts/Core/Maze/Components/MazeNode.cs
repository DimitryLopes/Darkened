using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MazeNode : Activateable
{
    //Generation
    private Coordinate coordinates;
    private bool hasTorch;

    public bool Visited { get; set; }
    public int X => coordinates.X;
    public int Y => coordinates.Y;
    public int ActiveWalls { get; private set; } = 0;
    public Coordinate Coordinates => coordinates;
    public Dictionary<Cardinal, bool> Edges => edges;

    private Dictionary<Cardinal, MazeWall> wallDictionary = new Dictionary<Cardinal, MazeWall>();
    private Dictionary<Cardinal, bool> edges = new Dictionary<Cardinal, bool>();

    public bool HasWall(Cardinal direction)
    {
        return wallDictionary[direction] && wallDictionary[direction].IsActive;
    }

    public void AddWall(Cardinal direction, MazeWall wall)
    {
        if (!HasWall(direction))
        {
            wallDictionary[direction] = wall;
            bool isWallAtBorder = edges[direction];
            PositionWall(direction, isWallAtBorder);
            ActiveWalls++;
            wall.Activate();
        }
        else
        {
            Debug.LogWarning("There is already a wall at X: " + coordinates.X + " Y: " + coordinates.Y + " " + direction.ToString());
        }
    }

    public void RemoveWall(Cardinal direction)
    {
        if (HasWall(direction))
        {
            ClearWall(direction);
            Debug.Log("Removed wall at " + wallDictionary[direction].transform.position);
        }
        else
        {
            Debug.LogWarning("There was no a wall to remove at X: " + coordinates.X + " Y: " + coordinates.Y + " " + direction.ToString());
        }
    }

    private void PositionWall(Cardinal direction, bool isWallAtBorder)
    {
        Vector3 offset = NodeUtils.GetWallPositionOffset(direction) * NodeUtils.NODE_SIZE / 2;
        wallDictionary[direction].transform.localPosition = transform.localPosition + offset;
        wallDictionary[direction].AlignWith(direction, isWallAtBorder);
    }

    protected override void OnDeactivate()
    {
        MazeUtils.ExecuteActionWithAllCardinals(ClearWall);
        MazeUtils.ExecuteActionWithAllCardinals(ClearEdge);
        ActiveWalls = 0;
        hasTorch = false;
        Visited = false;
        coordinates = new Coordinate();
    }

    private void ClearWall(Cardinal direction)
    {
        wallDictionary[direction].Deactivate();
        ActiveWalls--;
    }

    private void ClearWall(MazeWall wall)
    {
        if (wallDictionary.ContainsValue(wall))
        {
            wall.Deactivate();
            ActiveWalls--;
        }
        else
        {
            Debug.LogError("Node at [" + X + "," + Y + "] doesn't have a wall at: " + wall.transform.localPosition);
        }
    }

    private void ClearEdge(Cardinal cardinal)
    {
        edges[cardinal] = false;
    }

    public void SetEdge(Cardinal cardinal, bool isOnEdge = false)
    {
        edges[cardinal] = isOnEdge;
    }

    private void FillDictionary(Cardinal direction)
    {
        if (!wallDictionary.ContainsKey(direction))
        {
            wallDictionary.Add(direction, null);
        }
        else
        {
            wallDictionary[direction] = null;
        }
    }

    public void SetCoordinate(int x, int y)
    {
        
            MazeUtils.ExecuteActionWithAllCardinals(FillDictionary);
        coordinates = new Coordinate(x, y);
        text.text = x + "," + y;
    }

    //In Game
    [SerializeField]
    private TextMeshProUGUI text;
    [SerializeField]
    private SpriteRenderer floorRenderer;

    public void Visit()
    {
        floorRenderer.color = Color.green;
        Visited = true;
    }
}
