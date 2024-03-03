using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class LevelManager
{
    private MazeManager mazeManager;

    [Inject]
    public LevelManager(LevelDataBase levelDataBase, MazeManager mazeManager)
    {
        levelDatas = levelDataBase.LevelDatas;
        this.mazeManager = mazeManager;
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

    private int currentRandomLevelSize = 8;
    public void ChangeRandomMazeSize(int size)
    {
        currentRandomLevelSize = size;
    }

    public void StartRandomLevel()
    {
        MazeData randomLevelData = new MazeData(currentRandomLevelSize);
        LoadLevel(randomLevelData);
    }
    #endregion

    private void LoadLevel(MazeData data)
    {
        mazeManager.LoadMaze(data);
    }
}
