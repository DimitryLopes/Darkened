using UnityEngine;
using Zenject;

public class GameManager 
{
    private readonly LevelManager levelManager;

    public void StartGame()
    {
        levelManager.StartRandomLevel();
    }

    public void EndGame()
    {
        Debug.Log("Game ended!");
    }

    [Inject]
    public GameManager(LevelManager levelManager)
    {
        this.levelManager = levelManager;
    }
}
