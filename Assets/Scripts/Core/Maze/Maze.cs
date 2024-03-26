using UnityEngine;

public class Maze 
{
    private MazeData data;
    private MazeNode[,] nodes;
    public MazeData Data => data;
    public MazeNode[,] Nodes => nodes;
    public Maze(MazeData data)
    {
        this.data = data;
    }

    public void SetNodes(MazeNode[,] nodes)
    {
        this.nodes = nodes;
    }
}
