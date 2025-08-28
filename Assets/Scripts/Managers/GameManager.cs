using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class GameManager
{
    private readonly PersistenceManager percistenceManager;
    private readonly UnlockableManager unlockableManager;
    private readonly ObjectiveManager objectiveManager;
    private readonly InventoryManager inventoryManager;
    private readonly ScreenManager screenManager;
    private readonly EntityManager entityManager;
    private readonly LevelManager levelManager;
    private readonly AudioManager audioManager;
    private readonly MazeManager mazeManager;
    private readonly HUDManager hudManager;
    private readonly SignalBus signalBus;

    public static bool IsOnPhone;

    public GameManager(LevelManager levelManager, MazeManager mazeManager, ObjectiveManager objectiveManager, EntityManager entityManager,
        ScreenManager screenManager, HUDManager hudManager, AudioManager audioManager, CameraManager cameraManager, InventoryManager inventoryManager,
        PersistenceManager percistenceManager, UnlockableManager unlockableManager, SignalBus signalBus)
    {
        this.percistenceManager = percistenceManager;
        this.unlockableManager = unlockableManager;
        this.objectiveManager = objectiveManager;
        this.inventoryManager = inventoryManager;
        this.screenManager = screenManager;
        this.entityManager = entityManager;
        this.levelManager = levelManager;
        this.audioManager = audioManager;
        this.mazeManager = mazeManager;
        this.hudManager = hudManager;
        this.signalBus = signalBus;

        IsOnPhone = Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer;

        signalBus.Subscribe<OnMazeLoadFinishSignal>(OnMazeLoadFinish);
        signalBus.Subscribe<OnGameCompletedSignal>(OnGameCompleted);
        signalBus.Subscribe<OnMazeLoadStartedSignal>(OnMazeLoadStart);
        signalBus.Subscribe<OnNewGameStartedSignal>(OnNewGameStarted);
    }


    #region Game Start
    public void StartStoryGame(PresetLevelData data)
    {
        levelManager.StartStoryLevel(data);
    }

    public void ReplayStoryLevel()
    {
        levelManager.StartStoryLevel(levelManager.CurrentStoryLevel);
    }

    private void StartNextStoryLevel()
    {
        levelManager.StartNextStoryLevel();
    }

    public void StartCustomGame()
    {
        levelManager.StartCustomLevel();
    }

    public void StartRandomGame()
    {
        levelManager.StartRandomLevel();
    }
    #endregion

    private void OnNewGameStarted()
    {
        unlockableManager.OnConditionMet(UnlockCondition.StartUnlocked, false);
    }

    //called be main menu listener, there might be a better way to do this
    public void OnGameStarted()
    {
        percistenceManager.LoadGame();
    }

    public void ShowMainMenu()
    {
        objectiveManager.SetObjective(null);
        audioManager.PlayBGM(AudioKey.BGM_main_menu);

        var controller = new MainMenuScreenController(levelManager.LevelDataBase, OnLevelViewClicked);
        screenManager.Show<MainMenuScreen>(controller);
    }

    private void OnLevelViewClicked(PresetLevelData data)
    {
        levelManager.StartStoryLevel(data);
    }

    public void FinishGame(bool objectiveCompleted, bool isForced = true)
    {
        Player player = entityManager.GetPlayer();
        player.ToggleActing(false);
        
        entityManager.DeactivateEnemy(levelManager.CurrentLevelData.EnemyType);
        
        Objective currentObjective = objectiveManager.CurrentObjective;
        string screenMessage = objectiveCompleted ? currentObjective.Data.VictoryMessage : currentObjective.Data.DefeatMessage;

        inventoryManager.Clear();
        hudManager.HideHud();

        if (!isForced) return;

        GameFinishScreenController controller;

        switch (levelManager.CurrentLevelType)
        {
            case LevelType.Story:
                UnityAction nextLevel = levelManager.IsLastLevel(levelManager.CurrentStoryLevel.ID) || !objectiveCompleted ? null : StartNextStoryLevel;
                if (objectiveCompleted)
                {
                    levelManager.OnNextLevelUnlocked();
                }
                controller = new GameFinishScreenController(screenMessage, ReplayStoryLevel, ShowMainMenu, nextLevel);
                break;
            case LevelType.Random:
                controller = new GameFinishScreenController(screenMessage, StartRandomGame, ShowMainMenu);
                break;
            case LevelType.Custom:
                controller = new GameFinishScreenController(screenMessage, StartCustomGame, ShowMainMenu);
                break;
            default:
                controller = new GameFinishScreenController(screenMessage, StartCustomGame, ShowMainMenu);
                break;
        }

        screenManager.Show<UIGameFinishScreen>(controller);
    }

    private void OnGameCompleted(OnGameCompletedSignal signal)
    {
        FinishGame(signal.Won);
    }

    private void OnMazeLoadStart(OnMazeLoadStartedSignal signal)
    {
        if (entityManager.CurrentEnemyType != EnemyType.None)
        {
            Enemy enemy = entityManager.GetCurrentEnemy();
            enemy.Deactivate();
        }
    }

    private void OnMazeLoadFinish()
    {
        Player player = entityManager.GetPlayer();

        player.transform.position = mazeManager.CurrentStartingNode.transform.position;
        player.ResetPlayer();

        signalBus.Fire(new OnPlayerSpawnedSignal(player));

        if (levelManager.CurrentLevelData.EnemyType != EnemyType.None)
        {
            Enemy enemy = entityManager.GetEnemy(levelManager.CurrentLevelData.EnemyType, true);
            enemy.transform.position = mazeManager.EnemyStartingNode.transform.position;
            entityManager.ActivateEnemy(levelManager.CurrentLevelData.EnemyType);
            enemy.SetMaze(mazeManager.CurrentMaze);
            signalBus.Fire(new OnEnemySpawnedSignal(enemy));
        }

        audioManager.PlayBGM(AudioKey.BGM_in_game);
        hudManager.ShowHud();
    }
}