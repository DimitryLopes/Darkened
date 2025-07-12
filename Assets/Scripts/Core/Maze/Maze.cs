using System.Collections.Generic;

public class Maze
{
    private RandomLevelData data;
    private MazeNode[,] nodes;
    private Dictionary<Coordinate, MazeNode> edgeNodes;
    private Dictionary<Coordinate, MazeNode> cornerNodes;

    public RandomLevelData Data => data;
    public MazeNode[,] Nodes => nodes;
    public Dictionary<Coordinate, MazeNode> EdgeNodes => edgeNodes;
    public Dictionary<Coordinate, MazeNode> CornerNodes => cornerNodes;

    public List<MazeNode> UsedNodes;
    public List<MazeTorch> Torches;
    public Dictionary<Coordinate, MazeNode> NodesByCoordinate { get; private set; } = new();

    public Maze(RandomLevelData data)
    {
        UsedNodes = new();
        Torches = new();
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

        edgeNodes = MazeUtils.GetEdgeNodes(nodes, data);
        cornerNodes = MazeUtils.GetCornerNodes(nodes, data);
    }

    public void AddTorch(MazeNode node, MazeTorch torch)
    {
        Torches.Add(torch);
        node.AddTorch(torch);
        node.MarkAsUsed();
    }

    public void MarkNodeAsUsed(MazeNode node)
    {
        if (UsedNodes.Contains(node)) return;

        UsedNodes.Add(node);
        node.MarkAsUsed();
    }
}
