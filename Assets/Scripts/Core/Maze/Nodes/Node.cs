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
    public object UsedBy { get; set; }
    public Coordinate Coordinates => coordinates;
    public Vector3 Position => associatedTiles[4].Position;

    private Dictionary<Cardinal, MazeWall> wallDictionary = new Dictionary<Cardinal, MazeWall>();
    private Dictionary<Cardinal, bool> edges = new Dictionary<Cardinal, bool>();
    private Dictionary<Cardinal, bool> borders = new Dictionary<Cardinal, bool>();
    private List<TileData> associatedTiles;

    public Node(Coordinate coordinate, MazeSizeData sizeData)
    {
        Setup(coordinate, sizeData);
    }

    #region Tiles
    public void AssociateTiles(List<TileData> associatedTiles)
    {
        this.associatedTiles = associatedTiles;
    }

    public List<TileData> GetCardinalTiles(Cardinal direction)
    {
        List<TileData> tileDatas = new();
        switch (direction)
        {
            case Cardinal.North:
                associatedTiles.FindAll(t => t.Y == 2).ForEach(t => tileDatas.Add(t));
                break;
            case Cardinal.East:
                associatedTiles.FindAll(t => t.X == 2).ForEach(t => tileDatas.Add(t));
                break;
            case Cardinal.South:
                associatedTiles.FindAll(t => t.Y == 0).ForEach(t => tileDatas.Add(t));
                break;
            case Cardinal.West:
                associatedTiles.FindAll(t => t.X == 0).ForEach(t => tileDatas.Add(t));
                break;
        }
        return tileDatas;
    }

    #endregion

    public void Setup(Coordinate coordinate, MazeSizeData sizeData)
    {
        MazeUtils.ExecuteActionWithAllCardinals(FillDictionary);
        UsedBy = null;
        Visited = false;
        coordinates = coordinate;

        bool isOnWestBorder = X == 0;
        bool isOnEastBorder = X == sizeData.Width - 1;
        bool isOnNorthBorder = Y == sizeData.Height - 1;
        bool isOnSouthBorder = Y == 0;

        borders[Cardinal.West] = isOnWestBorder;
        borders[Cardinal.East] = isOnEastBorder;
        borders[Cardinal.South] = isOnSouthBorder;
        borders[Cardinal.North] = isOnNorthBorder;

        edges[Cardinal.West] = isOnWestBorder && (isOnNorthBorder || isOnSouthBorder);
        edges[Cardinal.East] = isOnEastBorder && (isOnNorthBorder || isOnSouthBorder);
        edges[Cardinal.South] = isOnSouthBorder && (isOnEastBorder || isOnWestBorder);
        edges[Cardinal.North] = isOnNorthBorder && (isOnEastBorder || isOnWestBorder);

        coordinates = new Coordinate(X, Y);
    }

    #region Walls
    public bool HasWall(Cardinal direction)
    {
        if(wallDictionary.ContainsKey(direction))
        {
            return wallDictionary[direction] != null && wallDictionary[direction].IsActive;
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
    /// this will only return a wall
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

    public bool IsOnEdge(Cardinal cardinal)
    {
        return edges[cardinal];
    }

    public bool IsOnEdge()
    {
        return edges[Cardinal.North] || edges[Cardinal.East] || edges[Cardinal.South] || edges[Cardinal.West];
    }

    public bool IsOnBorder(Cardinal cardinal)
    {
        return borders[cardinal];
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