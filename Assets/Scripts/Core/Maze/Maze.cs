using System.Collections.Generic;

public class Maze 
{
    private MazeData data;
    private MazeNode[,] nodes;
    public MazeData Data => data;
    public MazeNode[,] Nodes => nodes;
    public Dictionary<Coordinate, MazeNode> NodesByCoordinate { get; private set; }

    public Maze(MazeData data)
    {
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
}
