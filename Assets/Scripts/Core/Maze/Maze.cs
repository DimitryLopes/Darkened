using System.Collections.Generic;

public class Maze
{
    private LevelData data;
    private MazeNode[,] nodes;
    public LevelData Data => data;
    public MazeNode[,] Nodes => nodes;
    public Dictionary<Coordinate, MazeNode> NodesByCoordinate { get; private set; }

    public Maze(LevelData data)
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
