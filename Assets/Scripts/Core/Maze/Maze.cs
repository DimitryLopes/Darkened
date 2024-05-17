using System.Collections.Generic;
using UnityEngine;

public class Maze
{
    private LevelData data;
    private MazeNode[,] nodes;
    public LevelData Data => data;
    public MazeNode[,] Nodes => nodes;
    public List<MazeNode> UsedNodes;
    public List<MazeTorch> Torches;
    public Dictionary<Coordinate, MazeNode> NodesByCoordinate { get; private set; }

    public Maze(LevelData data)
    {
        UsedNodes = new();
        Torches = new();
        this.data = data;
    }

    public void SetNodes(MazeNode[,] nodes)
    {
        this.nodes = nodes;
        NodesByCoordinate = new Dictionary<Coordinate, MazeNode>();

        foreach(MazeNode node in nodes)
        {
            NodesByCoordinate.Add(node.Coordinates, node);
        }
    }

    public void AddTorch(MazeNode node, MazeTorch torch)
    {
        Torches.Add(torch);
        node.AddTorch(torch);
    }

    public void MarkNodeAsUsed(MazeNode node)
    {
        if (UsedNodes.Contains(node)) return;

        UsedNodes.Add(node);
        node.MarkAsUsed();
    }
}
