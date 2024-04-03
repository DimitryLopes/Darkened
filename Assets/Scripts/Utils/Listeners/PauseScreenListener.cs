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

        UIPauseScreen scrren = screenManager.GetScreen<UIPauseScreen>();
        if (scrren.IsShown)
        {
            scrren.Hide();
        }
        else
        {
            scrren.Show(new PauseScreenController());
        }
    }
}
