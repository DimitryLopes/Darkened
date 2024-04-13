using System;
using UnityEngine;

public class HUD : MonoBehaviour
{
    [SerializeField]
    private ObjectiveHUD objectiveHUD;
    [SerializeField]
    private PlayerHUD playerHUD;
    [SerializeField]
    private ItemHUD itemHUD;

    public ItemHUD ItemHUD => itemHUD;

    private void Start()
    {
        objectiveHUD.RawDeactivate();
        playerHUD.RawDeactivate();
        itemHUD.RawDeactivate();
    }

    public void Show()
    {
        objectiveHUD.Activate();
        playerHUD.Activate();
        itemHUD.Activate();
    }

    public void Hide()
    {
        objectiveHUD.Deactivate();
        playerHUD.Deactivate();
        itemHUD.Deactivate();
        itemHUD.Clear();
    }


}
