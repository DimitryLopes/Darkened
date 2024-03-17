using System.Collections.Generic;
using Zenject;

public class LevelManager
{
    private MazeManager mazeManager;
    private ObjectivesDataBase objectivesDataBase;
    private LevelDataBase levelDataBase;

    public LevelManager(LevelDataBase levelDataBase, MazeManager mazeManager, ObjectivesDataBase objectivesDataBase)
    {
        this.levelDataBase = levelDataBase;
        this.mazeManager = mazeManager;
        this.objectivesDataBase = objectivesDataBase;
    }

    #region Defined Level

    public void StartLevel(int levelIndex)
    {
        if(levelDataBase.LevelDatas[levelIndex].Data != null)
        {
            LoadLevel(levelDataBase.LevelDatas[levelIndex].Data);
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
        Objective baseObjective = objectivesDataBase.GetRandomObjective();
        Objective objective = Objective.CreateInstance<Objective>();
        objective.SetUp(baseObjective);

        //MazeTorch torch = levelDataBase.Torches.GetRandom();

        MazeData data = MazeData.CreateInstance<MazeData>();
        data.SetUp(currentRandomLevelSize, objective, ItemType.DefaultTorch);
        LoadLevel(data);
    }
    #endregion

    private void LoadLevel(MazeData data)
    {
        mazeManager.LoadMaze(data);
    }
}
