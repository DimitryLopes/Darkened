using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class MainMenuScreen : UIScreen<MainMenuScreenController>
{
    [Inject]
    private GameManager gameManager;

    [SerializeField]
    private Button startGameButton;

    private void Start()
    {
        startGameButton.onClick.AddListener(StartGame);
    }

    public void StartGame()
    {
        gameManager.StartGame();
    }
}
