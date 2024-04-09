using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "MazeData", menuName = "Scriptable Objects/Datas/Maze Data")]
public class MazeData : ScriptableObject, IUISelectable
{
    [SerializeField]
    private string levelName;

    [SerializeField, Header("Size")]
    private MazeSizeData sizeData;

    [SerializeField, Header("Difficulty")]
    private DifficultyData difficultyData;

    [SerializeField, Range(0,1), Header("Torches")]
    private float torchRatio;
    [SerializeField]
    private ItemType torch;

    [SerializeField, Header("Enemy")]
    private EnemyType enemyType;

    [SerializeField, Space]
    private ObjectiveData objective;

    public int Height => sizeData.Height;
    public int Width => sizeData.Width;
    public int Size => sizeData.Width * sizeData.Height;
    public MazeSizeData SizeData => sizeData;
    public DifficultyData DifficultyData => difficultyData;

    public int MinTorchCount => Mathf.CeilToInt(difficultyData.MinimumTorchRatio * Size);
    public int MaxTorchCount => Mathf.CeilToInt(difficultyData.MaximumTorchRatio * Size);
    public float TorchRatio => torchRatio;
    public ItemType Torch => torch;
    public EnemyType EnemyType => enemyType;
    public ObjectiveData ObjectiveData => objective;

    public string Title => levelName;
    public SelectableType SelectableType => SelectableType.Level;


    public void SetUp(MazeSizeData sizeData, ObjectiveData objective, DifficultyData difficulty, ItemType torch, EnemyType enemyType)
    {
        this.sizeData = sizeData;
        this.objective = objective;
        this.torch = torch;
        difficultyData = difficulty;
        torchRatio = (difficultyData.MinimumTorchRatio + difficultyData.MaximumTorchRatio) / 2;
        this.enemyType = enemyType;
    }

    public void SetUp(MazeData data, ObjectiveData objective)
    {
        SetUp(data.sizeData, objective, difficultyData, ItemType.DefaultTorch, data.enemyType);
    }
}
