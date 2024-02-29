using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField]
    private MazeWall wallPrefab;
    [SerializeField]
    private MazeNode nodePrefab;
    [SerializeField]
    private MazeData TemporaryDAta;

    [SerializeField, Header("Containers")]
    private Transform nodeContainer;
    [SerializeField]
    private Transform wallsContainer;

    private List<MazeWall> instantiatedWalls = new List<MazeWall>();


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            CreateMaze(TemporaryDAta);
        }
    }
    public void CreateMaze(MazeData data)
    {
        MazeNode[,] mazeNodes = CreateBase(data);
    }

    public MazeNode[,] CreateBase(MazeData data)
    {
        MazeNode[,] nodes = new MazeNode[data.Width, data.Height];
        for (int y = 0; y < data.Height; y++)
        {
            for (int x = 0; x < data.Width; x++)
            {
                MazeNode newNode = Instantiate(nodePrefab, nodeContainer);
                nodes[x, y] = newNode;
                newNode.SetCoordinate(x, y);
                PositionNode(newNode);
                MazeUtils.ExecuteActionWithAllCardinals(AddNodeWalls, newNode);
            }
        }
        return nodes;
    }

    private void AddNodeWalls(Cardinal direction, MazeNode newNode)
    {
        MazeWall wall = GetAvailableWall();
        newNode.AddWall(direction, wall);
    }

    private MazeWall GetAvailableWall()
    {
        foreach(MazeWall wall in instantiatedWalls)
        {
            if (!wall.IsActive)
            {
                return wall;
            }
        }
        MazeWall newWall = Instantiate(wallPrefab, wallsContainer);
        instantiatedWalls.Add(newWall);
        return newWall;
    }

    private void PositionNode(MazeNode node)
    {
        node.transform.position = new Vector2(node.X, node.Y);
    }
}
