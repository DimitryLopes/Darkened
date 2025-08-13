using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class LevelManager
{
    private MazeManager mazeManager;
    private UnlockableManager unlockableManager;
    private ObjectivesDataBase objectivesDataBase;
    private LevelDataBase levelDataBase;
    private MazeSizeDataBase mazeSizeDataBase;
    private EnemyDataBase enemyDataBase;
    private DifficultyDataBase difficultyDataBase;

    public LevelType CurrentLevelType { get; private set; }
    public LevelData CurrentLevelData { get; private set; }
    public LevelDataBase LevelDataBase => levelDataBase;

    public LevelManager(MazeManager mazeManager, LevelDataBase levelDataBase,
        ObjectivesDataBase objectivesDataBase, MazeSizeDataBase mazeSizeDataBase,
        SignalBus signalBus, EnemyDataBase enemyDataBase, DifficultyDataBase difficultyDataBase,
        UnlockableManager unlockableManager)
    {
        this.mazeManager = mazeManager;
        this.levelDataBase = levelDataBase;
        this.enemyDataBase = enemyDataBase;
        this.mazeSizeDataBase = mazeSizeDataBase;
        this.unlockableManager = unlockableManager;
        this.objectivesDataBase = objectivesDataBase;
        this.difficultyDataBase = difficultyDataBase;

        signalBus.Subscribe<OnSelectableSelectedSignal>(OnSelectableSelected);
    }

    public bool IsLevelUnlocked(int index)
    {
        return levelDataBase.LevelDatas[index].SavedData.IsUnlocked;
    }

    public List<LevelData> GetUnlockedLevels()
    {
        List<LevelData> unlockedDatas = new();
        foreach (LevelData data in levelDataBase.LevelDatas)
        {
            if (!data.SavedData.IsUnlocked) continue;

            unlockedDatas.Add(data);
        }

        if (unlockedDatas.Count > 0) return unlockedDatas;

        UnlockLevel(levelDataBase.LevelDatas[0]);
        unlockedDatas.Add(levelDataBase.LevelDatas[0]);
        return unlockedDatas;
    }

    private void UnlockLevel(LevelData data)
    {
        if (data.SavedData.IsUnlocked) return;

        unlockableManager.OnConditionMet(data.UnlockConditionData.UnlockCondition);
    }

    #region Story Level
    public PresetLevelData CurrentStoryLevel { get; private set; }
    public bool IsLastLevel(int levelID)
    {
        return levelID >= levelDataBase.LevelDatas.Count - 1;
    }

    public void StartStoryLevel(PresetLevelData data)
    {
        if(data != null)
        {
            LoadLevel(data, LevelType.Story);
            CurrentStoryLevel = data;
        }
    }

    public void OnNextLevelUnlocked()
    {
        if (IsLastLevel(CurrentStoryLevel.ID)) return;

        UnlockLevel(levelDataBase.LevelDatas[CurrentStoryLevel.ID + 1]);
    }

    public void StartNextStoryLevel()
    {
        if (IsLastLevel(CurrentStoryLevel.ID))
        {
            Debug.LogError("No more levels to load, hide the button developer");
            return;
        }

        StartStoryLevel(levelDataBase.LevelDatas[CurrentStoryLevel.ID + 1] as PresetLevelData);
    }

    #endregion

    #region Custom Level
    private MazeSizeData customSizeData;
    private ObjectiveType customObjectiveType = ObjectiveType.FindExit;
    private DifficultyData customDifficultyData;
    

    public void StartCustomLevel()
    {
        ObjectiveData objective = GetObjectiveByType(customObjectiveType);
        RandomLevelData data = RandomLevelData.CreateInstance<RandomLevelData>();
        data.SetUp(customSizeData, objective, customDifficultyData, EnemyType.Default);
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

        RandomLevelData data = RandomLevelData.CreateInstance<RandomLevelData>();
        data.SetUp(mazeSizeData, objective, difficultyData, enemyDataBase.EnemyTypes.GetRandom());
        LoadLevel(data, LevelType.Random);
    }

    #endregion
    private void OnSelectableSelected(OnSelectableSelectedSignal signal)
    {
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
        }
    }

    private void LoadLevel(LevelData data, LevelType levelType)
    {
        CurrentLevelType = levelType;
        CurrentLevelData = data;

        if(levelType == LevelType.Story)
        {
            ObjectiveData objective = GetObjectiveByType(data.ObjectiveData.ObjectiveType);
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
