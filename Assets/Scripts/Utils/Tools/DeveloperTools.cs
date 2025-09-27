using System.IO;
using UnityEngine;

public class DeveloperTools
{
    private MazeManager mazeManager;
    private EntityManager entityManager;
    private ObjectiveManager objectiveManager;
    private LevelManager levelManager;
    private PersistenceManager persistenceManager;

    public DeveloperTools(MazeManager mazeManager, ObjectiveManager objectiveManager, EntityManager entityManager,
        LevelManager levelManager, PersistenceManager persistenceManager)
    {
        this.mazeManager = mazeManager;
        this.objectiveManager = objectiveManager;
        this.entityManager = entityManager;
        this.levelManager = levelManager;
        this.persistenceManager = persistenceManager;
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
        entityManager.GetPlayer().ActivateTorch();
    }

    public void DeactivatePlayerTorch()
    {
        entityManager.GetPlayer().DeactivateTorch();
    }
    #endregion

    #region Maze
    public void ActivateAllTorches()
    {
        foreach (Torch torch in mazeManager.CurrentMaze.Torches)
        {
            torch.ActivateLights();
        }
        ActivatePlayerTorch();
    }

    public void DeactivateAllTorches()
    {
        foreach (Torch torch in mazeManager.CurrentMaze.Torches)
        {
            torch.DeactivateLights();
        }
        DeactivatePlayerTorch();
    }

    public void RegenerateMaze()
    {
        mazeManager.LoadMaze(levelManager.CurrentLevelData);
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

    public void UnlockEverything()
    {
        foreach(var persistence in persistenceManager.FindAllPersistences())
        {
            if(persistence is IUnlockable unlockable)
            {
                unlockable.Unlock();
            }
        }
        persistenceManager.Save();
    }
    #endregion
}
