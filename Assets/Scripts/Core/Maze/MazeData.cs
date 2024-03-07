using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "MazeData", menuName = "Scriptable Objects/Maze Data")]
public class MazeData : ScriptableObject
{
    [SerializeField]
    private int height;
    [SerializeField]
    private int width;
    [SerializeField]
    private float minDistanceStartToFinish;
    [SerializeField]
    private Objective objective;

    public int Height => height;
    public int Width => width;
    public Objective Objective => objective;
    public float MinDistanceStartToFinish => minDistanceStartToFinish;

    public void SetUp(int size, Objective objective)
    {
        height = size;
        width = size;
        this.objective = objective;
        minDistanceStartToFinish = Mathf.Ceil(size / 2);
    }

    public void StartObjective(SignalBus signalBus)
    {
        objective.StartObjective(signalBus);
    }
}
