using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class MainMenuScreen : UIScreen<MainMenuScreenController>
{
    [Inject]
    private GameManager gameManager;
    [Inject]
    private ScreenManager screenManager;

    [SerializeField]
    private Button storyModeButton;
    [SerializeField]
    private Button startCustomGameButton;
    [SerializeField]
    private Button startRandomGameButton;
    [SerializeField]
    private Button quitButton;

    private Action onAfterHideCallback;

    private void Start()
    {
        startCustomGameButton.onClick.AddListener(StartCustomGame);
        startRandomGameButton.onClick.AddListener(StartRandomGame);
        storyModeButton.onClick.AddListener(OnStoryModeButtonClicked);
        quitButton.onClick.AddListener(Quit);
    }

    private void OnStoryModeButtonClicked()
    {
        ShowLevelSelectionScreen();
    }

    private void ShowLevelSelectionScreen()
    {
        LevelSelectionScreenController controller = new LevelSelectionScreenController(Controller.LevelDataBase.LevelDatas, Controller.OnLevelViewClicked);
        screenManager.Show<UILevelSelectionScreen>(controller);
    }

    protected override void OnAfterHide()
    {
        base.OnAfterHide();
        onAfterHideCallback?.Invoke();
    }

    public void StartCustomGame()
    {
        onAfterHideCallback = gameManager.StartCustomGame;
        Hide();
    }

    public void StartRandomGame()
    {
        gameManager.StartRandomGame();
    }

    public void Quit()
    {
        Application.Quit();
    }
}
