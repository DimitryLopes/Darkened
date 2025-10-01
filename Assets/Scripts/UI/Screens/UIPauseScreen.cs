using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class UIPauseScreen : UIScreen<PauseScreenController>
{
    [Inject]
    private GameManager gameManager;
    [Inject]
    private TimeManager timeManager;

    [SerializeField]
    private Button quitButton;
    [SerializeField]
    private Button backButton;
    [SerializeField]
    private Button resumeButton;

    private void Start()
    {
        quitButton.onClick.AddListener(Quit);
        backButton.onClick.AddListener(ReturnToMenu);
        resumeButton.onClick.AddListener(Resume);
    }

    private void Resume()
    {
        Hide();
    }

    private void Quit()
    {
        Application.Quit();
    }

    private void ReturnToMenu()
    {
        gameManager.FinishGame(false, false);
        gameManager.ShowMainMenu();
    }

    protected override void OnBeforeShow()
    {
        base.OnBeforeShow();
        timeManager.Pause(this);
    }

    protected override void OnBeforeHide()
    {
        base.OnBeforeHide();
        timeManager.Resume(this);
    }
}
