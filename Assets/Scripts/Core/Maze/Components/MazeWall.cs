using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;

public class MazeWall : IToggable
{
    public List<TileData> AssociatedTiles { get; private set; }

    protected SignalBus signalBus;
    public bool IsAtBorder { get; protected set; }
    public bool IsActive => State != MazeWallState.Empty;
    public MazeWallState State { get; private set; }

    public (Node, Node) AdjacentNodes;
    public Vector3 Position => AssociatedTiles[1].Position;
    private bool toggled;
    public bool Toggled => toggled;

    public void AddNode(Node node)
    {
        if (AdjacentNodes.Item1 == null || AdjacentNodes.Item1 == node)
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

    public void SetAsClosedGate(Tilemap tilemap)
    {
        Vector3Int pos = new Vector3Int();
        foreach (TileData coordinate in AssociatedTiles)
        {
            pos.x = coordinate.X;
            pos.y = coordinate.Y;
            tilemap.SetTile(pos, AssetService.GetClosedGateTile());
        }
        State = MazeWallState.Gate_Closed;
    }

    public void SetAsOpenGate(Tilemap tilemap)
    {
        Vector3Int pos = new Vector3Int();
        foreach (TileData coordinate in AssociatedTiles)
        {
            pos.x = coordinate.X;
            pos.y = coordinate.Y;
            tilemap.SetTile(pos, AssetService.GetOpenGateTile());
        }
        State = MazeWallState.Gate_Open;
    }

    public void SetAsEmpty(Tilemap tilemap)
    {
        Vector3Int pos = new Vector3Int();
        foreach (TileData coordinate in AssociatedTiles)
        {
            pos.x = coordinate.X;
            pos.y = coordinate.Y; 
            tilemap.SetTile(pos, null);
        }
        State = MazeWallState.Empty;
    }

    public void Toggle(MazeManager mazeManager)
    {
        if (State == MazeWallState.Gate_Closed)
        {
            SetAsOpenGate(mazeManager.CurrentMaze.WallTilemap);
            toggled = true;
        }
        else if (State == MazeWallState.Gate_Open)
        {
            SetAsClosedGate(mazeManager.CurrentMaze.WallTilemap);
            toggled = false;
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
        Gate_Closed,
        Gate_Open
    }
}
