using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Node : Activateable
{
    //Generation
    private Coordinate coordinates;

    public bool Visited { get; set; }
    public int X => coordinates.X;
    public int Y => coordinates.Y;
    public bool IsOnCorner { get; private set; }
    public object UsedBy { get; set; }
    public Coordinate Coordinates => coordinates;

    private Dictionary<Cardinal, MazeWall> wallDictionary = new Dictionary<Cardinal, MazeWall>();
    private Dictionary<Cardinal, bool> edges = new Dictionary<Cardinal, bool>();

    public void Visit()
    {
        Visited = true;
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

    public bool HasAnyDefaultWall()
    {
        foreach (Cardinal cardinal in Enum.GetValues(typeof(Cardinal)))
        {
            if (wallDictionary[cardinal].IsActive && !wallDictionary[cardinal].GetType().IsSubclassOf(typeof(MazeWall)))
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

    public void ReplaceWall(Cardinal direction, MazeWall wall)
    {
        if (HasWall(direction))
        {
            bool isWallAtBorder = edges[direction];
            PositionWall(direction, isWallAtBorder);
            wallDictionary[direction] = wall;
        }
        else
        {
            Debug.LogWarning("There was no a wall to replace at X: " + coordinates.X + " Y: " + coordinates.Y + " " + direction);
        }
    }

    public void ActivateWall(Cardinal direction)
    {
        if (wallDictionary.ContainsKey(direction) && wallDictionary[direction] != null)
        {
            wallDictionary[direction].Activate();
        }
        else
        {
            Debug.LogWarning($"There is no wall to activate at X: { coordinates.X } Y: { coordinates.Y }  {direction}");
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
        if (wallDictionary[direction] == null || !wallDictionary[direction].IsActive) return;

        wallDictionary[direction].Deactivate();

    }

    /// <summary>
    /// this can also return special walls
    /// </summary>
    /// <returns></returns>
    public (MazeWall,Cardinal) GetAnyWall()
    {
        List<Cardinal> cardinals = EnumUtils.GetEnumValues<Cardinal>();
        cardinals.Shuffle();
        foreach (Cardinal cardinal in cardinals)
        {
            if (HasWall(cardinal))
            {
                return (GetWall(cardinal), cardinal);
            }
        }
        return (null, Cardinal.North);
    }
    /// <summary>
    /// this will only returns a wall
    /// </summary>
    /// <returns></returns>
    public (MazeWall, Cardinal) GetAnyDefaultWall()
    {
        return NodeUtils.GetAnyDefaultWall(this);
    }

    public (MazeWall, Cardinal) GetRandomWallOnEdge()
    {
        List<Cardinal> cardinals = EnumUtils.GetEnumValues<Cardinal>();
        cardinals.Shuffle();
        foreach (Cardinal cardinal in cardinals)
        {
            if (HasWall(cardinal) && IsOnEdge(cardinal))
            {
                return (GetWall(cardinal), cardinal);
            }
        }
        return (null, Cardinal.North);
    }
    #endregion

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

    public bool IsOnEdge(Cardinal cardinal)
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
        UsedBy = null;
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

    public float GetHeuristic(Node node)
    {
        return Vector3.Distance(transform.position, node.transform.position);
    }
    #endregion

    #region Debug

    public void DebugColor(Color color)
    {
        if (color == null) return;

        floorRenderer.color = color;
    }
    #endregion
}
