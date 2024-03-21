using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class LevelManager
{
    private MazeManager mazeManager;
    private ObjectivesDataBase objectivesDataBase;
    private LevelDataBase levelDataBase;
    private MazeSizeDataBase mazeSizeDataBase;

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
            LoadLevel(levelDataBase.LevelDatas[levelIndex].Data);
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
        Objective objective = objectivesDataBase.GetObjective(customObjectiveType);
        MazeData data = MazeData.CreateInstance<MazeData>();
        data.SetUp(customSizeData, objective, ItemType.DefaultTorch);
        LoadLevel(data);
    }

    #endregion

    #region Random Level

    public void StartRandomLevel()
    {
        Objective baseObjective = objectivesDataBase.GetRandomObjective();
        Objective objective = Objective.CreateInstance<Objective>();
        objective.SetUp(baseObjective);

        MazeSizeData mazeSizeData = mazeSizeDataBase.GetSizeDatas().GetRandom();
        //MazeTorch torch = levelDataBase.Torches.GetRandom();

        MazeData data = MazeData.CreateInstance<MazeData>();
        data.SetUp(mazeSizeData, objective, ItemType.DefaultTorch);
        LoadLevel(data);
    }

    #endregion

    private void LoadLevel(MazeData data)
    {
        mazeManager.LoadMaze(data);
        float cameraPos = data.SizeData.cameraPosition;
        Camera.main.transform.position = new Vector3(cameraPos, cameraPos, -10);
        Camera.main.orthographicSize = data.SizeData.cameraSize;
    }
}
