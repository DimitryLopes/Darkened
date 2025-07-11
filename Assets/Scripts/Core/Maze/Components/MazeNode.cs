using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class MazeNode : Activateable
{
    //Generation
    private Coordinate coordinates;
    private bool hasTorch;
    private bool isBeingUsed;

    public bool Visited { get; set; }
    public bool DeadEnd  { get; set; }
    public int X => coordinates.X;
    public int Y => coordinates.Y;
    public bool IsBeingUsed => isBeingUsed;
    public bool IsOnCorner { get; private set; }
    public Coordinate Coordinates => coordinates;

    private Dictionary<Cardinal, MazeWall> wallDictionary = new Dictionary<Cardinal, MazeWall>();
    private Dictionary<Cardinal, bool> edges = new Dictionary<Cardinal, bool>();

    public void Visit()
    {
        Visited = true;
    }

    public void MarkAsDeadEnd()
    {
        DeadEnd = true;
    }

    #region Walls
    public bool HasWall(Cardinal direction)
    {
        return wallDictionary[direction] && wallDictionary[direction].IsActive;
    }

    public bool HasAnyWall()
    {
        foreach (Cardinal cardinal in Enum.GetValues(typeof(Cardinal)))
        {
            if (wallDictionary[cardinal].IsActive)
            {
                return true;
            }
        }
        return false;
    }

    public void AddWall(Cardinal direction, MazeWall wall)
    {
        if (!HasWall(direction))
        {
            wallDictionary[direction] = wall;
            bool isWallAtBorder = edges[direction];
            PositionWall(direction, isWallAtBorder);
        }
        else
        {
            Debug.LogWarning("There is already a wall at X: " + X + " Y: " + Y + " " + direction);
        }
    }

    public void RemoveWall(Cardinal direction)
    {
        if (HasWall(direction))
        {
            ClearWall(direction);
        }
        else
        {
            Debug.LogWarning("There was no a wall to remove at X: " + coordinates.X + " Y: " + coordinates.Y + " " + direction);
        }
    }

    public MazeWall GetWall(Cardinal cardinal)
    {
        if (wallDictionary.ContainsKey(cardinal)) 
        {
            return wallDictionary[cardinal];
        }
        return null;
    }

    private void PositionWall(Cardinal direction, bool isWallAtBorder)
    {
        Vector3 offset = NodeUtils.GetWallPositionOffset(direction) * NodeUtils.NODE_SIZE / 2;
        wallDictionary[direction].transform.localPosition = transform.localPosition + offset;
        wallDictionary[direction].AlignWith(direction, isWallAtBorder);
    }

    private void ClearWall(Cardinal direction)
    {
        if (wallDictionary[direction].IsActive)
        {
            wallDictionary[direction].Deactivate();
        }
    }

    public int GetActiveWallCount()
    {
        int count = 0;
        MazeUtils.ExecuteActionWithAllCardinals(CountWall);
        return count;

        void CountWall(Cardinal cardinal)
        {
            if (!wallDictionary[cardinal].IsActive) return;
            count++;
        }
    }

    public (MazeWall, MazeWall) GetAdjacentWalls(Cardinal cardinal)
    {
        (Cardinal, Cardinal) tuple = NodeUtils.GetAdjacentCardinals(cardinal);
        (MazeWall, MazeWall) walls;
        walls.Item1 = GetWall(tuple.Item1);
        walls.Item2 = GetWall(tuple.Item2);
        return walls;
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
        }
    }

    private void AddTorchAt(MazeWall wall, MazeTorch torch)
    {
        wall.PositionObject(torch);
        torch.SetNode(this, wall.AlignedWith);
    }
    #endregion

    public void MarkAsUsed()
    {
        if (!IsBeingUsed)
        {
            isBeingUsed = true;
        }
    }

    #region Edges
    private void ClearEdgeAndCorner(Cardinal cardinal)
    {
        edges[cardinal] = false;
        IsOnCorner = false;
    }

    public void SetEdge(Cardinal cardinal, bool isOnEdge = false)
    {
        edges[cardinal] = isOnEdge;
    }

    public bool GetEdge(Cardinal cardinal)
    {
        return edges[cardinal];
    }

    public void SetCorner()
    {
        IsOnCorner = true;
    }

    #endregion

    public override void OnDeactivate()
    {
        MazeUtils.ExecuteActionWithAllCardinals(ClearWall);
        MazeUtils.ExecuteActionWithAllCardinals(ClearEdgeAndCorner);
        hasTorch = false;
        isBeingUsed = false;
        Visited = false;
        DeadEnd = false;
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

    #region In Game
    //In Game
    [SerializeField]
    private TextMeshProUGUI text;
    [SerializeField]
    private SpriteRenderer floorRenderer;
    #endregion

    #region Path Finding
    public float gScore { get; set; }
    public float fScore { get; set; }

    public float GetHeuristic(MazeNode node)
    {
        return Vector3.Distance(transform.position, node.transform.position);
    }
    #endregion

    #region Debug
    public void ShowIsDeadEnd()
    {
        text.text = DeadEnd ? "DEAD END" : string.Empty;
    }

    public void ShowWallCount()
    {
        text.text = GetActiveWallCount().ToString();
    }
    public void DebugColor(Color color)
    {
        if (color == null) return;

        floorRenderer.color = color;
    }
    #endregion
}
