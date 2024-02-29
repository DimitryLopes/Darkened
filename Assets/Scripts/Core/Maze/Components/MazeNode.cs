using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeNode : MonoBehaviour
{
    //Generation
    private Coordinate coordinate;
    private bool hasTorch;

    public bool Visited { get; set; }
    public int X => coordinate.X;
    public int Y => coordinate.Y;

    private Dictionary<Cardinal, MazeWall> wallDictionary = new Dictionary<Cardinal, MazeWall>();

    public bool HasWall(Cardinal direction)
    {
        return wallDictionary[direction];
    }

    public void AddWall(Cardinal direction, MazeWall wall)
    {
        if (!HasWall(direction))
        {
            wallDictionary[direction] = wall;
            PositionWall(direction);
            wall.Activate();
        }
        else
        {
            Debug.LogWarning("There is already a wall at X: " + coordinate.X + " Y: " + coordinate.Y + " " + direction.ToString());
        }
    }

    public void RemoveWall(Cardinal direction)
    {
        if (HasWall(direction))
        {
            wallDictionary[direction].Dectivate();
        }
        else
        {
            Debug.LogWarning("There was no a wall to remove at X: " + coordinate.X + " Y: " + coordinate.Y + " " + direction.ToString());
        }
    }

    private void PositionWall(Cardinal direction)
    {
        Vector3 offset = NodeUtils.GetWallPositionOffset(direction) * NodeUtils.NODE_SIZE / 2;
        wallDictionary[direction].transform.position = transform.position + offset;
    }

    public void Clear()
    {
        MazeUtils.ExecuteActionWithAllCardinals(ClearWall);
        coordinate = new Coordinate();
    }

    private void ClearWall(Cardinal direction)
    {
        wallDictionary[direction].Dectivate();
    }

    private void FillDictionary(Cardinal direction)
    {
        wallDictionary.Add(direction, null);
    }

    public void SetCoordinate(int x, int y)
    {
        MazeUtils.ExecuteActionWithAllCardinals(FillDictionary);
        coordinate = new Coordinate(x, y);
    }

    //In Game
    [SerializeField]
    private SpriteRenderer floorRenderer;
}
