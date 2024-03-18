using UnityEngine;
using Zenject;

public class MainMenuListener : MonoBehaviour
{
    [Inject]
    private ScreenManager screenManager;

    void Start()
    {
        MainMenuScreen screen = screenManager.GetScreen<MainMenuScreen>();
        screen.Show(new MainMenuScreenController());
    }
}
