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

    public List<MazeNode> UsedNodes = new();
    public List<MazeTorch> Torches = new();
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
            NodesByCoordinate.Add(node.Coordinates, node);
        }

        edgeNodes = MazeUtils.GetEdgeNodes(nodes, data.SizeData);
        cornerNodes = MazeUtils.GetCornerNodes(nodes, data.SizeData);
    }

    public void AddTorch(MazeNode node, MazeTorch torch, bool preset = false, Cardinal direction = Cardinal.North)
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
        node.MarkAsUsed();
    }

    public void MarkNodeAsUsed(MazeNode node)
    {
        if (UsedNodes.Contains(node)) return;

        UsedNodes.Add(node);
        node.MarkAsUsed();
    }
}
