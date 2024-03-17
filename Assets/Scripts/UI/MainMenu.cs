using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class MainMenu : MonoBehaviour
{
    [Inject]
    private GameManager gameManager;
    private ScreenManager manager;

    public void StartGame()
    {
        UIPauseScreen screen = manager.GetScreen<UIPauseScreen>();
        gameManager.StartGame();
    }
}
