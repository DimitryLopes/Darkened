using System.Collections.Generic;

public class Maze
{
    private LevelData data;
    private Node[,] nodes;
    private Dictionary<Coordinate, Node> edgeNodes;
    private Dictionary<Coordinate, Node> cornerNodes;

    public LevelData Data => data;
    public Node[,] Nodes => nodes;
    public Dictionary<Coordinate, Node> EdgeNodes => edgeNodes;
    public Dictionary<Coordinate, Node> CornerNodes => cornerNodes;

    public List<Node> FreeNodes { get; set; } = new();
    public List<Node> UsedNodes { get; set; } = new();
    public List<Torch> Torches = new();
    public List<IToggable> Toggables { get; set; } = new();
    public Dictionary<Coordinate, Node> NodesByCoordinate { get; private set; } = new();

    public Maze(LevelData data)
    {
        this.data = data;
    }

    public void SetNodes(Node[,] nodes)
    {
        this.nodes = nodes;
        NodesByCoordinate.Clear();

        foreach(Node node in nodes)
        {
            FreeNodes.Add(node);
            NodesByCoordinate.Add(node.Coordinates, node);
        }

        edgeNodes = MazeUtils.GetEdgeNodes(nodes, data.SizeData);
        cornerNodes = MazeUtils.GetCornerNodes(nodes, data.SizeData);
    }

    public void AddTorch(Node node, Torch torch, Cardinal direction)
    {
        Torches.Add(torch);
        torch.Setup(direction);
        MarkNodeAsUsed(node, torch);
    }

    public void MarkNodeAsUsed(Node node, object usedBy)
    {
        
            UnityEngine.Debug.Log($"Node {node.name} is being marked as used by {usedBy}");

        if (!FreeNodes.Contains(node))
        {
            UnityEngine.Debug.Log($"Node {node.Coordinates} is already used or not found in FreeNodes.");
            return;
        }

        UsedNodes.Add(node);
        FreeNodes.Remove(node);
    }
}
