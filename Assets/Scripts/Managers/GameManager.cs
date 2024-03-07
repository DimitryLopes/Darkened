using UnityEngine;
using Zenject;

public class GameManager 
{
    private readonly LevelManager levelManager;
    private readonly ObjectiveManager objectiveManager;
    private readonly MazeManager mazeManager;
    private readonly SignalBus signalBus;

    public void StartGame()
    {
        levelManager.StartRandomLevel();
    }

    public void FinishGame()
    {
        Debug.Log("Game ended!");
    }

    [Inject]
    public GameManager(LevelManager levelManager, MazeManager mazeManager, ObjectiveManager objectiveManager, SignalBus signalBus)
    {
        this.objectiveManager = objectiveManager;
        this.levelManager = levelManager;
        this.mazeManager = mazeManager;
        this.signalBus = signalBus;

        signalBus.Subscribe<OnObjectiveCompletedSignal>(OnObjectiveCompleted);
    }

    private void OnObjectiveCompleted(OnObjectiveCompletedSignal signal)
    {
        if(signal.Objective == objectiveManager.CurrentObjective)
        {
            FinishGame();
        }
    }
}
