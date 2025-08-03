using UnityEngine;

[CreateAssetMenu(fileName = "Rename Me", menuName = "Scriptable Objects/Datas/Maze Size Data")]
public class MazeSizeData : ScriptableObject, IUISelectable
{
    [SerializeField]
    private int height;
    [SerializeField]
    private int width;
    [SerializeField, Header("Items")]
    private float averageAdditionalItemAmount;
    [SerializeField, Header("Gates")]
    private float averageGateAmount;

    public int Width => width;
    public int Height => height;


    public float AverageGateAmount => averageGateAmount;
    public float AverageAdditionalItemAmount => averageAdditionalItemAmount;

    public SelectableType SelectableType => SelectableType.MazeSize;

    public string Title => $"{Width} x {Height}";
}
