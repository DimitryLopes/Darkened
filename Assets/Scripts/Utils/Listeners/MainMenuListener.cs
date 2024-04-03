using UnityEngine;
using Zenject;

public class MainMenuListener : MonoBehaviour
{
    [Inject]
    private GameManager gameManager;

    void Start()
    {
        gameManager.ShowMainMenu();
    }
}
