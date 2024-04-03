using UnityEngine;
using Zenject;

public class LevelManager
{
    private MazeManager mazeManager;
    private ObjectivesDataBase objectivesDataBase;
    private LevelDataBase levelDataBase;
    private MazeSizeDataBase mazeSizeDataBase;
    private EnemyDataBase enemyDataBase;
    private DifficultyDataBase difficultyDataBase;

    public LevelType CurrentLevelType { get; private set; }
    public MazeData CurrentLevelData { get; private set; }
    
    public LevelManager(MazeManager mazeManager, LevelDataBase levelDataBase,
        ObjectivesDataBase objectivesDataBase, MazeSizeDataBase mazeSizeDataBase,
        SignalBus signalBus, EnemyDataBase enemyDataBase, DifficultyDataBase difficultyDataBase)
    {
        this.mazeManager = mazeManager;
        this.levelDataBase = levelDataBase;
        this.enemyDataBase = enemyDataBase;
        this.mazeSizeDataBase = mazeSizeDataBase;
        this.objectivesDataBase = objectivesDataBase;
        this.difficultyDataBase = difficultyDataBase;

        signalBus.Subscribe<OnSelectableSelectedSignal>(OnSelectableSelected);
    }

    #region Story Level

    public void StartStoryLevel(int levelIndex)
    {
        if(levelDataBase.LevelDatas[levelIndex].Data != null)
        {
            LoadLevel(levelDataBase.LevelDatas[levelIndex].Data, LevelType.Story);
        }
    }

    #endregion

    #region Custom Level
    private MazeSizeData customSizeData;
    private ObjectiveType customObjectiveType = ObjectiveType.FindExit;
    private DifficultyData customDifficultyData;
    private void OnSelectableSelected(OnSelectableSelectedSignal signal)
    {
        switch (signal.Selectable.SelectableType)
        {
            case SelectableType.MazeSize:
                MazeSizeData mazeSizeData = signal.Selectable as MazeSizeData;
                customSizeData = mazeSizeData;
                //currentRandomLevelWidth = data.Width;
                break;
            case SelectableType.Difficulty:
                DifficultyData difficultyData = signal.Selectable as DifficultyData;
                customDifficultyData = difficultyData;
                break;
            case SelectableType.Objective:
                ObjectiveData objective = signal.Selectable as ObjectiveData;
                customObjectiveType = objective.ObjectiveType;
                break;
        }
    }

    public void StartCustomLevel()
    {
        ObjectiveData objective = GetObjectiveByType(customObjectiveType);
        MazeData data = MazeData.CreateInstance<MazeData>();
        data.SetUp(customSizeData, objective, customDifficultyData, ItemType.DefaultTorch, EnemyType.Default);
        LoadLevel(data, LevelType.Custom);
    }

    #endregion

    #region Random Level
    public void StartRandomLevel()
    {
        ObjectiveData objective = GetObjective();
        MazeSizeData mazeSizeData = mazeSizeDataBase.GetSizeDatas().GetRandom();
        DifficultyData difficultyData = difficultyDataBase.GetRandomData();
        //MazeTorch torch = levelDataBase.Torches.GetRandom();

        MazeData data = MazeData.CreateInstance<MazeData>();
        data.SetUp(mazeSizeData, objective, difficultyData, ItemType.DefaultTorch, enemyDataBase.EnemyTypes.GetRandom());
        LoadLevel(data, LevelType.Random);
    }

    #endregion

    private void LoadLevel(MazeData data, LevelType levelType)
    {
        CurrentLevelType = levelType;
        CurrentLevelData = data;

        if(levelType == LevelType.Story)
        {
            ObjectiveData objective = GetObjectiveByType(data.ObjectiveData.ObjectiveType);
            data.SetUp(data, objective);
        }
        mazeManager.LoadMaze(data);

        float cameraPos = data.SizeData.cameraPosition;
        Camera.main.transform.position = new Vector3(cameraPos, cameraPos, -10);
        Camera.main.orthographicSize = data.SizeData.cameraSize;
    }

    private ObjectiveData GetObjective(ObjectiveData targetObjective = null)
    {
        ObjectiveData objectiveData;
        if (targetObjective != null)
        {
            objectiveData = objectivesDataBase.GetObjective(targetObjective.ObjectiveType);
        }
        else
        {
            objectiveData = objectivesDataBase.GetRandomObjective();
        }
        return objectiveData;
    }

    private ObjectiveData GetObjectiveByType(ObjectiveType type)
    {
        ObjectiveData baseObjective;
        baseObjective = objectivesDataBase.GetObjective(type);
        baseObjective = GetObjective(baseObjective);
        return baseObjective;
    }
}

public enum LevelType
{
    Story,
    Custom,
    Random,
}
