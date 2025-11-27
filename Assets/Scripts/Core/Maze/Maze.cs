using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class Maze
{
    private LevelData data;
    private Node[,] nodes;
    private Dictionary<Coordinate, Node> edgeNodes;
    private Dictionary<Coordinate, Node> borderNodes;

    public LevelData Data => data;
    public Node[,] Nodes => nodes;
    public Dictionary<Coordinate, Node> EdgeNodes => edgeNodes;
    public Dictionary<Coordinate, Node> BorderNodes => borderNodes;

    public List<Node> FreeNodes { get; set; } = new();
    public List<Node> UsedNodes { get; set; } = new();
    public List<Torch> Torches = new();
    public List<IToggable> Toggables { get; set; } = new();
    public Tilemap WallTilemap { get; private set; }
    public Dictionary<Coordinate, Node> NodesByCoordinate { get; private set; } = new();

    public Maze(LevelData data, Tilemap wallsTilemap)
    {
        this.data = data;
        WallTilemap = wallsTilemap;
    }

    public void SetNodes(Node[,] nodes)
    {
        this.nodes = nodes;
        NodesByCoordinate.Clear();

        foreach(Node node in nodes)
        {
            FreeNodes.Add(node);
            NodesByCoordinate.Add(node.Coordinate, node);
        }

        edgeNodes = MazeUtils.GetEdgeNodes(nodes, data.SizeData);
        borderNodes = MazeUtils.GetBorderNodes(nodes, data.SizeData);
    }

    public void AddTorch(Node node, Torch torch, Cardinal direction)
    {
        Torches.Add(torch);
        torch.Setup(direction);
        MarkNodeAsUsed(node, torch);
    }

    public void MarkNodeAsUsed(Node node, object usedBy)
    {
        UnityEngine.Debug.Log($"Node [{node.X},{node.Y}]  is being marked as used by {usedBy}");

        if (!FreeNodes.Contains(node))
        {
            UnityEngine.Debug.Log($"Node {node.Coordinate} is already used or not found in FreeNodes.");
            return;
        }
        node.UsedBy = usedBy;
        UsedNodes.Add(node);
        FreeNodes.Remove(node);
    }
}
