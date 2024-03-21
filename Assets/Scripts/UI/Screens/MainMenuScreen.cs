using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class MainMenuScreen : UIScreen<MainMenuScreenController>
{
    [Inject]
    private GameManager gameManager;

    [SerializeField]
    private Button startStoryGameButton;
    [SerializeField]
    private Button startCustomGameButton;
    [SerializeField]
    private Button startRandomGameButton;

    private void Start()
    {
        startStoryGameButton.onClick.AddListener(StartStoryGame);
        startCustomGameButton.onClick.AddListener(StartCustomGame);
        startRandomGameButton.onClick.AddListener(StartRandomGame);
    }

    public void StartStoryGame()
    {
        gameManager.StartStoryGame();
    }
    public void StartCustomGame()
    {
        gameManager.StartCustomGame();
    }
    public void StartRandomGame()
    {
        gameManager.StartRandomGame();
    }
}
