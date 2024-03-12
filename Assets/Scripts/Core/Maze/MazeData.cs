using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "MazeData", menuName = "Scriptable Objects/Maze Data")]
public class MazeData : ScriptableObject
{

    [SerializeField, Header("Size")]
    private int height;
    [SerializeField]
    private int width;

    [SerializeField, Range(0,1), Header("Torches")]
    private float minTorchRatio;
    [SerializeField, Range(0,1)]
    private float maxTorchRatio;
    [SerializeField, Range(0,1)]
    private float torchRatio;
    [SerializeField]
    private ItemType torch;

    [SerializeField, Space]
    private float minDistanceStartToFinish;
    [SerializeField]
    private Objective objective;

    public int Height => height;
    public int Width => width;
    public int MazeSize => width * height;

    public int MinTorchCount => (int)(MazeSize * minTorchRatio); 
    public int MaxTorchCount => (int)(MazeSize * maxTorchRatio); 
    public float TorchRatio => torchRatio; 
    
    public Objective Objective => objective;
    public ItemType Torch => torch;

    public float MinDistanceStartToFinish => minDistanceStartToFinish;

    public void SetUp(int size, Objective objective, ItemType torch)
    {
        height = size;
        width = size;
        this.objective = objective;
        this.torch = torch;
        //arbritary stupidity made by sky
        minTorchRatio = 0.15f; // ~30% of total nodes
        maxTorchRatio = 0.3f; // ~50% of total nodes
        torchRatio = (minTorchRatio + maxTorchRatio) / 2;
        minDistanceStartToFinish = Mathf.Ceil(size / 2);
    }

    public void StartObjective(SignalBus signalBus)
    {
        objective.StartObjective(signalBus);
    }
}
