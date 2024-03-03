using UnityEngine;

[CreateAssetMenu(fileName = "MazeData", menuName = "Scriptable Objects/Maze Data")]
public class MazeData : ScriptableObject
{
    [SerializeField]
    private int height;
    [SerializeField]
    private int width;
    [SerializeField]
    private float minDistanceStartToFinish;

    public int Height => height;
    public int Width => width;
    public float MinDistanceStartToFinish => minDistanceStartToFinish;

    public MazeData(int size)
    {
        height = size;
        width = size;
        minDistanceStartToFinish = Mathf.Ceil(size/2);
    }
}
