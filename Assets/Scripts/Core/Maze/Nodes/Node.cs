using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Node : Activateable
{
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    //Generation
    private Coordinate coordinates;

    public int X => coordinates.X;
    public int Y => coordinates.Y;

    public object UsedBy { get; set; }
    public Coordinate Coordinates => coordinates;

    private Dictionary<Cardinal, bool> edges = new Dictionary<Cardinal, bool>();

    public void Setup(int x, int y, MazeSizeData sizeData)
    {
        MazeUtils.ExecuteActionWithAllCardinals(ClearEdge);
        switch (x, y)
        {
            case (0, 0):
                spriteRenderer.sprite = AssetService.GetMazeFloor(MazeFloorType.bottomleft);
                edges[Cardinal.South] = true;
                edges[Cardinal.West] = true;
                break;
            case (0, var yy) when yy == sizeData.Height - 1:
                spriteRenderer.sprite = AssetService.GetMazeFloor(MazeFloorType.topleft);
                edges[Cardinal.North] = true;
                edges[Cardinal.East] = true;
                break;
            case (var xx, 0) when xx == sizeData.Width - 1:
                spriteRenderer.sprite = AssetService.GetMazeFloor(MazeFloorType.bottomright);
                edges[Cardinal.South] = true;
                edges[Cardinal.East] = true;
                break;
            case (var xx, var yy) when xx == sizeData.Width - 1 && yy == sizeData.Height - 1:
                spriteRenderer.sprite = AssetService.GetMazeFloor(MazeFloorType.topright);
                edges[Cardinal.North] = true;
                edges[Cardinal.West] = true;
                break;
            case (var xx, _) when xx == 0:
                spriteRenderer.sprite = AssetService.GetMazeFloor(MazeFloorType.left);
                edges[Cardinal.West] = true;
                break;
            case (var xx, _) when xx == sizeData.Width - 1:
                spriteRenderer.sprite = AssetService.GetMazeFloor(MazeFloorType.right);
                edges[Cardinal.East] = true;
                break;
            case (_, var yy) when yy == 0:
                spriteRenderer.sprite = AssetService.GetMazeFloor(MazeFloorType.bottom);
                edges[Cardinal.South] = true;
                break;
            case (_, var yy) when yy == sizeData.Height - 1:
                spriteRenderer.sprite = AssetService.GetMazeFloor(MazeFloorType.top);
                edges[Cardinal.North] = true;
                break;
            default:
                spriteRenderer.sprite = AssetService.GetMazeFloor(MazeFloorType.middle);
                break;
        }
        coordinates = new Coordinate(x, y);
        text.text = x + "," + y;
    }

    #region Edges
    public bool IsOnEdge(Cardinal cardinal)
    {
        return edges[cardinal];
    }

    private void ClearEdge(Cardinal cardinal)
    {
        edges[cardinal] = false;
    }

    #endregion

    public override void OnDeactivate()
    {
        UsedBy = null;
        Visited = false;
    }


    #region Path Finding
    public float gScore { get; set; }
    public float fScore { get; set; }

    public bool Visited { get; set; }
    public void Visit()
    {
        Visited = true;
    }
    public float GetHeuristic(Node node)
    {
        return Vector3.Distance(transform.position, node.transform.position);
    }
    #endregion

    #region Debug

    [SerializeField]
    private TextMeshProUGUI text;
    public void DebugColor(Color color)
    {
        if (color == null) return;

        spriteRenderer.color = color;
    }
    #endregion
}
