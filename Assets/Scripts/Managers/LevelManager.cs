using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class LevelManager
{
    private MazeManager mazeManager;

    [Inject]
    public LevelManager(LevelDataBase levelDataBase, MazeManager mazeManager, ObjectivesDataBase objectivesDataBase)
    {
        levelDatas = levelDataBase.LevelDatas;
        this.mazeManager = mazeManager;
        this.objectivesDataBase = objectivesDataBase;
    }

    #region Defined Level
    List<LevelData> levelDatas;

    public void StartLevel(int levelIndex)
    {
        if(levelDatas[levelIndex].Data != null)
        {
            LoadLevel(levelDatas[levelIndex].Data);
        }
    }

    #endregion

    #region Random Level

    private ObjectivesDataBase objectivesDataBase;

    private int currentRandomLevelSize = 1024;

    public void ChangeRandomMazeSize(int size)
    {
        currentRandomLevelSize = size;
    }

    public void StartRandomLevel()
    {
        Objective objective = objectivesDataBase.GetRandomObjective();
        MazeData randomLevelData = new MazeData(currentRandomLevelSize, objective);
        LoadLevel(randomLevelData);
    }
    #endregion

    private void LoadLevel(MazeData data)
    {
        mazeManager.LoadMaze(data);
    }
}
