using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;

public class MazeWall 
{
    public List<TileData> AssociatedTiles { get; private set; }

    protected SignalBus signalBus;
    public bool IsAtBorder { get; protected set; }
    public bool IsActive => State != MazeWallState.Empty;
    public MazeWallState State { get; private set; }

    public (Node, Node) AdjacentNodes;


    public void AddNode(Node node)
    {
        if(AdjacentNodes.Item1 == null || AdjacentNodes.Item1 == node)
        {
            AdjacentNodes.Item1 = node;
        }
        else if (AdjacentNodes.Item2 == null || AdjacentNodes.Item2 == node)
        {
            AdjacentNodes.Item2 = node;
        }
        else
        {
            Debug.LogError("Cannot add more than two adjacent nodes to a wall.");
        }
    }

    public void Setup(List<TileData> associatedTiles, bool isAtBorder)
    {
        AssociatedTiles = associatedTiles;
        IsAtBorder = isAtBorder;
    }

    public void SetAsWall(Tilemap tilemap)
    {
        Vector3Int pos = new Vector3Int();
        foreach (TileData coordinate in AssociatedTiles)
        {
            pos.x = coordinate.X;
            pos.y = coordinate.Y;
            tilemap.SetTile(pos, AssetService.GetWallTile());
        }
        State = MazeWallState.Wall;
    }

    public void SetAsGate(Tilemap tilemap)
    {
        Vector3Int pos = new Vector3Int();
        foreach (TileData coordinate in AssociatedTiles)
        {
            pos.x = coordinate.X;
            pos.y = coordinate.Y;
            tilemap.SetTile(pos, AssetService.GetGateTile());
        }
        State = MazeWallState.Gate;
    }

    public void SetAsEmpty(Tilemap tilemap)
    {
        Vector3Int pos = new Vector3Int();
        foreach (TileData coordinate in AssociatedTiles)
        {
            pos.x = coordinate.X;
            pos.y = coordinate.Y;
            tilemap.SetTile(pos, AssetService.GetFloorTile());
        }
        State = MazeWallState.Empty;
    }

    //public virtual void OnWallCreated(SignalBus signalBus)
    //{
    //    this.signalBus = signalBus;
    //    signalBus.Subscribe<OnItemsLoadFinishSignal>(AddToComposite);
    //}

    //private void AddToComposite()
    //{
    //    boxCollider.usedByComposite = true;
    //    signalBus.Unsubscribe<OnItemsLoadFinishSignal>(AddToComposite);
    //}
}

public enum MazeWallState
{
    Empty,
    Wall,
    Gate
}
