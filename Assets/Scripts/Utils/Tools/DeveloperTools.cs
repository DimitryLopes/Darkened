using System.IO;
using UnityEngine;

public class DeveloperTools
{
    private MazeManager mazeManager;
    private EntityManager entityManager;
    private ObjectiveManager objectiveManager;
    private LevelManager levelManager;

    public DeveloperTools(MazeManager mazeManager, ObjectiveManager objectiveManager, EntityManager entityManager,
        LevelManager levelManager)
    {
        this.mazeManager = mazeManager;
        this.objectiveManager = objectiveManager;
        this.entityManager = entityManager;
        this.levelManager = levelManager;
    }

    #region Enemy
    public void DisableEnemy()
    {
        Enemy enemy = GetEnemy();
        enemy.Deactivate();
    }

    public void EnableEnemy()
    {
        Enemy enemy = GetEnemy();
        enemy.Activate();
    }

    public void DisableEnemyCollision()
    {
        Enemy enemy = GetEnemy();
        enemy.DeactivateCollider();
    }

    public void EnableEnemyCollision()
    {
        Enemy enemy = GetEnemy();
        enemy.ActivateCollider();
    }

    private Enemy GetEnemy()
    {
        return entityManager.GetEnemy(mazeManager.CurrentMaze.Data.EnemyType);
    }
    #endregion

    #region Objective
    public void CompleteCurrentObjective()
    {
        objectiveManager.CompleteCurrentObjective();
    }

    public void CompleteCurrentMission()
    {
        objectiveManager.CompleteCurrentMission();
    }
    #endregion

    #region Player 
    public void ActivatePlayerTorch()
    {
        entityManager.GetPlayer().ActiveTorch();
    }

    public void DeactivatePlayerTorch()
    {
        entityManager.GetPlayer().DeactivateTorch();
    }
    #endregion

    #region Maze
    public void ActivateAllTorches()
    {
        foreach (MazeTorch torch in mazeManager.CurrentMaze.Torches)
        {
            torch.ActivateLights();
        }
        ActivatePlayerTorch();
    }

    public void DeactivateAllTorches()
    {
        foreach (MazeTorch torch in mazeManager.CurrentMaze.Torches)
        {
            torch.DeactivateLights();
        }
        DeactivatePlayerTorch();
    }

    public void RegenerateMaze()
    {
        mazeManager.LoadMaze(levelManager.CurrentLevelData);
    }

    public void ShowDeadEnds()
    {
        foreach (MazeNode node in mazeManager.CurrentMaze.Nodes)
        {
            node.ShowIsDeadEnd();
        }
    }

    public void ShowNodeWallCount()
    {
        foreach (MazeNode node in mazeManager.CurrentMaze.Nodes)
        {
            node.ShowWallCount();
        }
    }
    #endregion

    #region Persistence

    public void ErasePlayerData()
    {
        string path = Constants.Save.PERSISTENCE_FILE_PATH;
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("Deleted file at: " + path);
        }
    }
    #endregion
}
