using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class LevelManager
{
    private MazeManager mazeManager;
    private ObjectivesDataBase objectivesDataBase;
    private LevelDataBase levelDataBase;
    private MazeSizeDataBase mazeSizeDataBase;

    public LevelType CurrentLevelType { get; private set; }

    public LevelManager(MazeManager mazeManager, LevelDataBase levelDataBase,
        ObjectivesDataBase objectivesDataBase, MazeSizeDataBase mazeSizeDataBase,
        SignalBus signalBus)
    {
        this.mazeManager = mazeManager;
        this.levelDataBase = levelDataBase;
        this.mazeSizeDataBase = mazeSizeDataBase;
        this.objectivesDataBase = objectivesDataBase;

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
    private void OnSelectableSelected(OnSelectableSelectedSignal signal)
    {
        switch (signal.Selectable.Type)
        {
            case SelectableType.MazeSize:
                MazeSizeData data = signal.Selectable as MazeSizeData;
                customSizeData = data;
                //currentRandomLevelWidth = data.Width;
                break;
            case SelectableType.GameMode:
                break;
            case SelectableType.Objective:
                Objective objective = signal.Selectable as Objective;
                customObjectiveType = objective.ObjectiveType;
                break;
        }
    }

    public void StartCustomLevel()
    {
        Objective objective = GetObjectiveByType(customObjectiveType);
        MazeData data = MazeData.CreateInstance<MazeData>();
        data.SetUp(customSizeData, objective, ItemType.DefaultTorch);
        LoadLevel(data, LevelType.Custom);
    }

    #endregion

    #region Random Level
    public void StartRandomLevel()
    {
        Objective objective = GetObjective();
        MazeSizeData mazeSizeData = mazeSizeDataBase.GetSizeDatas().GetRandom();
        //MazeTorch torch = levelDataBase.Torches.GetRandom();

        MazeData data = MazeData.CreateInstance<MazeData>();
        data.SetUp(mazeSizeData, objective, ItemType.DefaultTorch);
        LoadLevel(data, LevelType.Random);
    }

    #endregion

    private void LoadLevel(MazeData data, LevelType levelType)
    {
        CurrentLevelType = levelType;

        if(levelType == LevelType.Story)
        {
            Objective objective = GetObjectiveByType(data.Objective.ObjectiveType);
            data.SetUp(data, objective);
        }
        mazeManager.LoadMaze(data);

        float cameraPos = data.SizeData.cameraPosition;
        Camera.main.transform.position = new Vector3(cameraPos, cameraPos, -10);
        Camera.main.orthographicSize = data.SizeData.cameraSize;
    }

    private Objective GetObjective(Objective targetObjective = null)
    {
        Objective baseObjective;
        if (targetObjective != null)
        {
            baseObjective = objectivesDataBase.GetObjective(targetObjective.ObjectiveType);
        }
        else
        {
            baseObjective = objectivesDataBase.GetRandomObjective();
        }

        Objective objective = Objective.CreateInstance<Objective>();
        objective.SetUp(baseObjective);
        return objective;
    }

    private Objective GetObjectiveByType(ObjectiveType type)
    {
        Objective baseObjective;
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
