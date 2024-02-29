using UnityEngine;

public class Maze : MonoBehaviour
{
    private MazeData data;
    private MazeNode[,] nodes;

    public void Create(MazeNode[,] nodes)
    {
        this.nodes = nodes;
    }
}
