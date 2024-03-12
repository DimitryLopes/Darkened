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

    private Dictionary<Cardinal, MazeWall> wallDictionary = new Dictionary<Cardinal, MazeWall>();
    private Dictionary<Cardinal, bool> edges = new Dictionary<Cardinal, bool>();
    
    #region Walls
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
            Debug.LogWarning("There is already a wall at X: " + X + " Y: " + Y + " " + direction.ToString());
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

    public MazeWall GetWall(Cardinal cardinal)
    {
        return wallDictionary[cardinal];
    }

    private void PositionWall(Cardinal direction, bool isWallAtBorder)
    {
        Vector3 offset = NodeUtils.GetWallPositionOffset(direction) * NodeUtils.NODE_SIZE / 2;
        wallDictionary[direction].transform.localPosition = transform.localPosition + offset;
        wallDictionary[direction].AlignWith(direction, isWallAtBorder);
    }

    private void ClearWall(Cardinal direction)
    {
        wallDictionary[direction].Deactivate();
        ActiveWalls--;
    }

    private void ClearWall(MazeWall wall)
    {
        wall.Deactivate();
        ActiveWalls--;
    }

    public MazeWall GetRandomWall()
    {
        return NodeUtils.GetRandomWall(this);
    }

    public MazeWall GetRandomWallOnEdge()
    {
        return NodeUtils.GetRandomWall(this, true);
    }
    #endregion

    #region Torches
    public void AddTorch(MazeTorch torch)
    {
        if (!hasTorch)
        {
            MazeWall wall = GetRandomWall();
            AddTorchAt(wall, torch);
            hasTorch = true;
            Debug.Log("Added torch at [" + X + "," + Y + "] at " + wall.AlignedWith + " wall");
        }
    }

    private void AddTorchAt(MazeWall wall, MazeTorch torch)
    {
        wall.PositionObject(torch);
    }
    #endregion

    #region Edges
    private void ClearEdge(Cardinal cardinal)
    {
        edges[cardinal] = false;
    }

    public void SetEdge(Cardinal cardinal, bool isOnEdge = false)
    {
        edges[cardinal] = isOnEdge;
    }

    public bool GetEdge(Cardinal cardinal)
    {
        return edges[cardinal];
    }

    #endregion

    protected override void OnDeactivate()
    {
        MazeUtils.ExecuteActionWithAllCardinals(ClearWall);
        MazeUtils.ExecuteActionWithAllCardinals(ClearEdge);
        ActiveWalls = 0;
        hasTorch = false;
        Visited = false;
        coordinates = new Coordinate();
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
