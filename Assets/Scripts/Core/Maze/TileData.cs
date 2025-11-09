using UnityEngine;
using UnityEngine.Tilemaps;

public struct TileData
{
    private TileBase tile;
    private Vector3Int position;

    public TileBase Tile => tile;
    public int X => position.x;
    public int Y => position.y;
    public Vector3Int Position => position;

    public TileData(TileBase tile, Vector3Int position)
    {
        this.tile = tile;
        this.position = position;
    }
}
