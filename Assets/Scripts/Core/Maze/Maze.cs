using System.Collections.Generic;

public class Maze
{
    private LevelData data;
    private MazeNode[,] nodes;
    private Dictionary<Coordinate, MazeNode> edgeNodes;
    private Dictionary<Coordinate, MazeNode> cornerNodes;

    public LevelData Data => data;
    public MazeNode[,] Nodes => nodes;
    public Dictionary<Coordinate, MazeNode> EdgeNodes => edgeNodes;
    public Dictionary<Coordinate, MazeNode> CornerNodes => cornerNodes;

    public List<MazeNode> FreeNodes { get; set; } = new();
    public List<MazeNode> UsedNodes { get; set; } = new();
    public List<Torch> Torches = new();
    public Dictionary<Coordinate, MazeNode> NodesByCoordinate { get; private set; } = new();

    public Maze(LevelData data)
    {
        this.data = data;
    }

    public void SetNodes(MazeNode[,] nodes)
    {
        this.nodes = nodes;
        NodesByCoordinate.Clear();

        foreach(MazeNode node in nodes)
        {
            FreeNodes.Add(node);
            NodesByCoordinate.Add(node.Coordinates, node);
        }

        edgeNodes = MazeUtils.GetEdgeNodes(nodes, data.SizeData);
        cornerNodes = MazeUtils.GetCornerNodes(nodes, data.SizeData);
    }

    public void AddTorch(MazeNode node, Torch torch, bool preset = false, Cardinal direction = Cardinal.North)
    {
        Torches.Add(torch);
        if (preset)
        {
            MazeWall wall = NodeUtils.GetWallAt(node, direction, this);
            node.AddTorchAt(wall, torch);
        }
        else
        {
            node.AddTorch(torch);
        }
        UsedNodes.Add(node);
        FreeNodes.Remove(node);
    }

    public void MarkNodeAsUsed(MazeNode node)
    {
        if (!FreeNodes.Contains(node))
        {
            UnityEngine.Debug.Log($"Node {node.Coordinates} is already used or not found in FreeNodes.");
            return;
        }

        UsedNodes.Add(node);
        FreeNodes.Remove(node);
    }
}
