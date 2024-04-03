using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class PauseScreenListener : MonoBehaviour
{
    [Inject]
    private ScreenManager screenManager;

    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape)) return;

        UIPauseScreen scrren = screenManager.GetScreen<UIPauseScreen>();
        scrren.Show(new PauseScreenController());
    }
}
