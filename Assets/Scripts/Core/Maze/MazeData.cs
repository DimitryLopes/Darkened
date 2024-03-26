using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "MazeData", menuName = "Scriptable Objects/Datas/Maze Data")]
public class MazeData : ScriptableObject
{

    [SerializeField, Header("Size")]
    private MazeSizeData sizeData;

    [SerializeField, Range(0,1), Header("Torches")]
    private float minTorchRatio;
    [SerializeField, Range(0,1)]
    private float maxTorchRatio;
    [SerializeField, Range(0,1)]
    private float torchRatio;
    [SerializeField]
    private ItemType torch;

    [SerializeField, Space]
    private ObjectiveData objective;

    public int Height => sizeData.Height;
    public int Width => sizeData.Width;
    public int Size => sizeData.Width * sizeData.Height;
    public MazeSizeData SizeData => sizeData;

    public int MinTorchCount => (int)(Size * minTorchRatio); 
    public int MaxTorchCount => (int)(Size * maxTorchRatio); 
    public float TorchRatio => torchRatio; 
    
    public ObjectiveData ObjectiveData => objective;
    public ItemType Torch => torch;


    public void SetUp(MazeSizeData sizeData, ObjectiveData objective, ItemType torch)
    {
        this.sizeData = sizeData;
        this.objective = objective;
        this.torch = torch;
        //arbritary stupidity made by sky
        minTorchRatio = 0.15f; // ~30% of total nodes
        maxTorchRatio = 0.3f; // ~50% of total nodes
        torchRatio = (minTorchRatio + maxTorchRatio) / 2;
    }

    public void SetUp(MazeData data, ObjectiveData objective)
    {
        SetUp(data.sizeData, objective, ItemType.DefaultTorch);
    }
}
