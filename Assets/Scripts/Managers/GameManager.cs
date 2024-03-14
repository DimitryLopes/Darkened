using UnityEngine;
using Zenject;

public class GameManager 
{
    private readonly ObjectiveManager objectiveManager;
    private readonly EntityManager entityManager;
    private readonly LevelManager levelManager;
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
    public GameManager(LevelManager levelManager, MazeManager mazeManager, ObjectiveManager objectiveManager, EntityManager entityManager,
        SignalBus signalBus)
    {
        this.objectiveManager = objectiveManager;
        this.entityManager = entityManager;
        this.levelManager = levelManager;
        this.mazeManager = mazeManager;
        this.signalBus = signalBus;

        signalBus.Subscribe<OnMazeLoadFinishSignal>(OnMazeLoadFinish);
        signalBus.Subscribe<OnObjectiveCompletedSignal>(OnObjectiveCompleted);
    }

    private void OnObjectiveCompleted(OnObjectiveCompletedSignal signal)
    {
        if(signal.Objective == objectiveManager.CurrentObjective)
        {
            FinishGame();
        }
    }
    
    private void OnMazeLoadFinish()
    {
        Player player = entityManager.GetPlayer();
        player.transform.position = mazeManager.CurrentStartingNode.transform.position;
        player.ToggleActing(true);
    }
}
