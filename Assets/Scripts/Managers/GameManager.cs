using Zenject;

public class GameManager
{
    private readonly PersistenceManager percistenceManager;
    private readonly ObjectiveManager objectiveManager;
    private readonly InventoryManager inventoryManager;
    private readonly ScreenManager screenManager;
    private readonly EntityManager entityManager;
    private readonly LevelManager levelManager;
    private readonly AudioManager audioManager;
    private readonly MazeManager mazeManager;
    private readonly HUDManager hudManager;

    public GameManager(LevelManager levelManager, MazeManager mazeManager, ObjectiveManager objectiveManager, EntityManager entityManager,
        ScreenManager screenManager, HUDManager hudManager, AudioManager audioManager, CameraManager cameraManager, InventoryManager inventoryManager,
        PersistenceManager percistenceManager, SignalBus signalBus)
    {
        this.percistenceManager = percistenceManager;
        this.objectiveManager = objectiveManager;
        this.inventoryManager = inventoryManager;
        this.screenManager = screenManager;
        this.entityManager = entityManager;
        this.levelManager = levelManager;
        this.audioManager = audioManager;
        this.mazeManager = mazeManager;
        this.hudManager = hudManager;

        signalBus.Subscribe<OnMazeLoadFinishSignal>(OnMazeLoadFinish);
        signalBus.Subscribe<OnGameCompletedSignal>(OnObjectiveCompleted);
    }


    #region Game Start
    public void StartStoryGame()
    {
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

    public void OnGameStarted()
    {
        percistenceManager.LoadGame();
    }

    //called be main menu listener, there might be a better way to do this
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
                controller = new GameFinishScreenController(screenMessage, StartStoryGame, ShowMainMenu);
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
        percistenceManager.SaveGame();

        screen.Show(controller);
    }

    private void OnObjectiveCompleted(OnGameCompletedSignal signal)
    {
        FinishGame(signal.Won);
    }

    private void OnMazeLoadFinish()
    {
        Player player = entityManager.GetPlayer();

        player.transform.position = mazeManager.CurrentStartingNode.transform.position;
        player.ToggleActing(true);

        Enemy enemy = entityManager.GetEnemy(levelManager.CurrentLevelData.EnemyType);
        enemy.transform.position = mazeManager.EnemyStartingNode.transform.position;
        entityManager.ActivateEnemy(levelManager.CurrentLevelData.EnemyType);
        enemy.SetMaze(mazeManager.CurrentMaze);

        audioManager.PlayBGM(AudioKey.BGM_in_game);
        hudManager.ShowHud();
    }
}