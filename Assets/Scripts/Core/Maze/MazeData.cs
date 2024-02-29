using UnityEngine;

[CreateAssetMenu(fileName = "MazeData", menuName = "Scriptable Objects/Maze Data")]
public class MazeData : ScriptableObject
{
    [SerializeField]
    private int height;
    [SerializeField]
    private int width;

    public int Height => height;
    public int Width => width;
}
