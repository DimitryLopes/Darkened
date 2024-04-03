using UnityEngine;

public class HUD : MonoBehaviour
{
    [SerializeField]
    private ObjectiveHUD objectiveHUD;
    [SerializeField]
    private PlayerHUD playerHUD;

    private void Start()
    {
        objectiveHUD.RawDeactivate();
        playerHUD.RawDeactivate();
    }

    public void Show()
    {
        objectiveHUD.Activate();
        playerHUD.Activate();
    }

    public void Hide()
    {
        objectiveHUD.Deactivate();
        playerHUD.Deactivate();
    }
}
