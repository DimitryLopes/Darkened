using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class LevelManager
{
    private MazeManager mazeManager;
    private PersistenceManager percistenceManager;
    private ObjectivesDataBase objectivesDataBase;
    private LevelDataBase levelDataBase;
    private MazeSizeDataBase mazeSizeDataBase;
    private EnemyDataBase enemyDataBase;
    private DifficultyDataBase difficultyDataBase;

    public LevelType CurrentLevelType { get; private set; }
    public LevelData CurrentLevelData { get; private set; }
    
    public LevelManager(MazeManager mazeManager, LevelDataBase levelDataBase,
        ObjectivesDataBase objectivesDataBase, MazeSizeDataBase mazeSizeDataBase,
        SignalBus signalBus, EnemyDataBase enemyDataBase, DifficultyDataBase difficultyDataBase,
        PersistenceManager saveManager)
    {
        this.mazeManager = mazeManager;
        this.percistenceManager = saveManager;
        this.levelDataBase = levelDataBase;
        this.enemyDataBase = enemyDataBase;
        this.mazeSizeDataBase = mazeSizeDataBase;
        this.objectivesDataBase = objectivesDataBase;
        this.difficultyDataBase = difficultyDataBase;

        signalBus.Subscribe<OnSelectableSelectedSignal>(OnSelectableSelected);
    }


    public List<LevelData> GetUnlockedLevels()
    {
        List<LevelData> unlockedDatas = new();
        foreach (LevelData data in levelDataBase.LevelDatas)
        {
            if (data.SavedData.IsUnlocked)
            {
                unlockedDatas.Add(data);
            }
        }
        return unlockedDatas;
    }

    #region Story Level
    private int currentStoryLevelIndex = 0;
    public void StartStoryLevel()
    {
        if(levelDataBase.LevelDatas[currentStoryLevelIndex] != null)
        {
            LoadLevel(levelDataBase.LevelDatas[currentStoryLevelIndex], LevelType.Story);
        }
    }
    #endregion

    #region Custom Level
    private MazeSizeData customSizeData;
    private ObjectiveType customObjectiveType = ObjectiveType.FindExit;
    private DifficultyData customDifficultyData;
    private void OnSelectableSelected(OnSelectableSelectedSignal signal)
    {
        Debug.Log(signal.Selectable.Title + " Selected");
        switch (signal.Selectable.SelectableType)
        {
            case SelectableType.MazeSize:
                MazeSizeData mazeSizeData = signal.Selectable as MazeSizeData;
                customSizeData = mazeSizeData;
                //currentRandomLevelWidth = data.Width;
                return;
            case SelectableType.Difficulty:
                DifficultyData difficultyData = signal.Selectable as DifficultyData;
                customDifficultyData = difficultyData;
                return;
            case SelectableType.Objective:
                ObjectiveData objective = signal.Selectable as ObjectiveData;
                customObjectiveType = objective.ObjectiveType;
                return;
            case SelectableType.Level:
                LevelData levelData = signal.Selectable as LevelData;
                currentStoryLevelIndex = levelDataBase.GetLevelID(levelData);
                return;
        }
    }

    public void StartCustomLevel()
    {
        ObjectiveData objective = GetObjectiveByType(customObjectiveType);
        LevelData data = LevelData.CreateInstance<LevelData>();
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

        LevelData data = LevelData.CreateInstance<LevelData>();
        data.SetUp(mazeSizeData, objective, difficultyData, ItemType.DefaultTorch, enemyDataBase.EnemyTypes.GetRandom());
        LoadLevel(data, LevelType.Random);
    }

    #endregion

    private void LoadLevel(LevelData data, LevelType levelType)
    {
        CurrentLevelType = levelType;
        CurrentLevelData = data;

        if(levelType == LevelType.Story)
        {
            ObjectiveData objective = GetObjectiveByType(data.ObjectiveData.ObjectiveType);
            data.SetUp(data, objective);
        }
        mazeManager.LoadMaze(data);
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
