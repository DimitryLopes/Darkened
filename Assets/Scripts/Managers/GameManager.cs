using System;
using UnityEngine;
using Zenject;

public class GameManager 
{
    private readonly ObjectiveManager objectiveManager;
    private readonly ScreenManager screenManager;
    private readonly EntityManager entityManager;
    private readonly LevelManager levelManager;
    private readonly MazeManager mazeManager;
    private readonly SignalBus signalBus;

    public void StartStoryGame()
    {
        HideMainMenu();
        levelManager.StartStoryLevel(0);
    }

    public void StartCustomGame()
    {
        HideMainMenu();
        levelManager.StartCustomLevel();

    }

    public void StartRandomGame()
    {
        HideMainMenu();
        levelManager.StartRandomLevel();
    }

    private void HideMainMenu()
    {
        MainMenuScreen screen = screenManager.GetScreen<MainMenuScreen>();
        screen.Hide();
    }

    public void FinishGame()
    {
        Debug.Log("Game ended!");
    }

    [Inject]
    public GameManager(LevelManager levelManager, MazeManager mazeManager, ObjectiveManager objectiveManager, EntityManager entityManager,
        ScreenManager screenManager, SignalBus signalBus)
    {
        this.objectiveManager = objectiveManager;
        this.screenManager = screenManager;
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
