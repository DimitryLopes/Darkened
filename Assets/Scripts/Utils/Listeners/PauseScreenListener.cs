using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class PauseScreenListener : MonoBehaviour
{
    [Inject]
    private ScreenManager screenManager;

    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape) || screenManager.IsShowingScreen) return;

        UIPauseScreen screen = screenManager.GetScreen<UIPauseScreen>();
        if (screen.IsShown)
        {
            screen.Hide();
        }
        else
        {
            screenManager.Show<UIPauseScreen>(new PauseScreenController());
        }
    }
}
