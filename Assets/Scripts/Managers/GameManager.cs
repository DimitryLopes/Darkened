using Zenject;

public class GameManager
{
    private readonly ObjectiveManager objectiveManager;
    private readonly ScreenManager screenManager;
    private readonly EntityManager entityManager;
    private readonly LevelManager levelManager;
    private readonly MazeManager mazeManager;
    private readonly SignalBus signalBus;

    #region Game Start
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
    #endregion

    private void HideMainMenu()
    {
        var screen = screenManager.GetScreen<MainMenuScreen>();
        screen.Hide();
    }

    private void ShowMainMenu()
    {
        objectiveManager.SetObjective(null);

        var screen = screenManager.GetScreen<MainMenuScreen>();
        var controller = new MainMenuScreenController();
        screen.Show(controller);
    }

    public void FinishGame(bool objectiveCompleted)
    {
        Player player = entityManager.GetPlayer();
        player.ToggleActing(false);
        entityManager.DeactivateEnemy(levelManager.CurrentLevelData.EnemyType);


        Objective currentObjective = objectiveManager.CurrentObjective;

        string screenMessage = objectiveCompleted ? currentObjective.Data.VictoryMessage : currentObjective.Data.DefeatMessage;
        string screenTitle = currentObjective.Data.Title;

        var screen = screenManager.GetScreen<UIGameFinishScreen>();
        GameFinishScreenController controller;
        switch (levelManager.CurrentLevelType)
        {
            case LevelType.Story:
                controller = new GameFinishScreenController(screenMessage, screenTitle, StartStoryGame, ShowMainMenu);
                break;
            case LevelType.Random:
                controller = new GameFinishScreenController(screenMessage, screenTitle, StartRandomGame, ShowMainMenu);
                break;
            case LevelType.Custom:
                controller = new GameFinishScreenController(screenMessage, screenTitle, StartCustomGame, ShowMainMenu);
                break;
            default:
                controller = new GameFinishScreenController(screenMessage, screenTitle, StartCustomGame, ShowMainMenu);
                break;
        }
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
    }

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
        signalBus.Subscribe<OnGameCompletedSignal>(OnObjectiveCompleted);
    }
}
