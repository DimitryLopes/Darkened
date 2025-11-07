using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Node
{
    //Generation
    private Coordinate coordinates;

    public int X => coordinates.X;
    public int Y => coordinates.Y;
    public bool IsOnCorner { get; private set; }
    public object UsedBy { get; set; }
    public Coordinate Coordinates => coordinates;

    private Dictionary<Cardinal, MazeWall> wallDictionary = new Dictionary<Cardinal, MazeWall>();
    private Dictionary<Cardinal, bool> edges = new Dictionary<Cardinal, bool>();

    public Node(Coordinate coordinate, MazeSizeData sizeData)
    {
        Setup(coordinate, sizeData);
    }

    public void Setup(Coordinate coordinate, MazeSizeData sizeData)
    {
        MazeUtils.ExecuteActionWithAllCardinals(ClearEdgeAndCorner);
        UsedBy = null;
        Visited = false;
        coordinates = coordinate;
        switch (X, Y)
        {
            case (0, 0):
                edges[Cardinal.South] = true;
                edges[Cardinal.West] = true;
                break;
            case (0, var yy) when yy == sizeData.Height - 1:
                edges[Cardinal.North] = true;
                edges[Cardinal.East] = true;
                break;
            case (var xx, 0) when xx == sizeData.Width - 1:
                edges[Cardinal.South] = true;
                edges[Cardinal.East] = true;
                break;
            case (var xx, var yy) when xx == sizeData.Width - 1 && yy == sizeData.Height - 1:
                edges[Cardinal.North] = true;
                edges[Cardinal.West] = true;
                break;
            case (var xx, _) when xx == 0:
                edges[Cardinal.West] = true;
                break;
            case (var xx, _) when xx == sizeData.Width - 1:
                edges[Cardinal.East] = true;
                break;
            case (_, var yy) when yy == 0:
                edges[Cardinal.South] = true;
                break;
            case (_, var yy) when yy == sizeData.Height - 1:
                edges[Cardinal.North] = true;
                break;
            default:
                break;
        }
        coordinates = new Coordinate(X, Y);
    }
    
    #region Walls
    public bool HasWall(Cardinal direction)
    {
        return wallDictionary[direction].IsActive;
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
            wallDictionary[direction] = wall;
        }
        else
        {
            Debug.LogWarning("There was no a wall to replace at X: " + coordinates.X + " Y: " + coordinates.Y + " " + direction);
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

    private void ClearWall(Cardinal direction)
    {


    }

    /// <summary>
    /// this can also return special walls
    /// </summary>
    /// <returns></returns>
    public (MazeWall, Cardinal) GetAnyWall()
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


    #region Path Finding
    public float gScore { get; set; }
    public float fScore { get; set; }

    public bool Visited { get; set; }
    public void Visit()
    {
        Visited = true;
    }
    public float GetHeuristic(Node a)
    {
        return Mathf.Abs(X - a.X) + Mathf.Abs(Y - a.Y);
    }
    #endregion
}