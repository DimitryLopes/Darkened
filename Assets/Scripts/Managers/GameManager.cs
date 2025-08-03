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
        signalBus.Subscribe<OnGameCompletedSignal>(OnObjectiveCompleted);
        signalBus.Subscribe<OnMazeLoadStartedSignal>(OnMazeLoadStart);
        signalBus.Subscribe<OnNewGameStartedSignal>(OnNewGameStarted);
    }


    #region Game Start
    public void StartStoryGame()
    {
        levelManager.StartStoryLevel();
    }

    private void StartNextStoryLevel()
    {
        levelManager.IncreaseLevelIndex();
        levelManager.StartStoryLevel();
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

        var screen = screenManager.GetScreen<MainMenuScreen>();
        var controller = new MainMenuScreenController();
        screen.Show(controller);
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

        var screen = screenManager.GetScreen<UIGameFinishScreen>();
        GameFinishScreenController controller;

        switch (levelManager.CurrentLevelType)
        {
            case LevelType.Story:
                UnityAction nextLevel = levelManager.IsLastLevel || !objectiveCompleted ? null : StartNextStoryLevel;
                if (objectiveCompleted)
                {
                    levelManager.OnNextLevelUnlocked();
                }
                controller = new GameFinishScreenController(screenMessage, StartStoryGame, ShowMainMenu, nextLevel);
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

        screen.Show(controller);
    }

    private void OnObjectiveCompleted(OnGameCompletedSignal signal)
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
        player.ToggleActing(true);
        player.SetDefaultTorch();

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