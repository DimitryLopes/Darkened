using UnityEngine;

public class HUD : MonoBehaviour
{
    [SerializeField]
    private ObjectiveHUD objectiveHUD;
    [SerializeField]
    private PlayerHUD playerHUD;
    [SerializeField]
    private ItemHUD itemHUD;
    [SerializeField]
    private BottomHUD bottomHUD;

    public ItemHUD ItemHUD => itemHUD;
    public BottomHUD BottomHUD => bottomHUD;

    private void Start()
    {
        objectiveHUD.RawDeactivate();
        bottomHUD.RawDeactivate();
        playerHUD.RawDeactivate();
        itemHUD.RawDeactivate();
    }

    public void Show()
    {
        objectiveHUD.Activate();
        bottomHUD.Activate();
        playerHUD.Activate();
        itemHUD.Activate();
    }

    public void Hide()
    {
        objectiveHUD.Deactivate();
        bottomHUD.Deactivate();
        playerHUD.Deactivate();
        itemHUD.Deactivate();
        itemHUD.Clear();
    }
}
