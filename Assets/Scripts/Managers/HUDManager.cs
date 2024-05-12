using Zenject;
using System;

public class HUDManager
{
    private HUD hud;


    [Inject]
    public HUDManager(HUD hud)
    {
        this.hud = hud;
    }

    public void ShowHud()
    {
        hud.Show();
        UpdateBottomHUD(false);
    }

    public void HideHud()
    {
        hud.Hide();
    }

    public void CreateItemViews(int amount)
    {
        hud.ItemHUD.CreateRawViews(amount);
    }

    public void CreateItemView(InventoryItemData itemData)
    {
        hud.ItemHUD.CreateItemView(itemData);
    }

    public void UpdateItemView(InventoryItemData data)
    {
        hud.ItemHUD.UpdateItemView(data);
    }

    public void UpdateBottomHUD(bool hasUse)
    {
        hud.BottomHUD.UpdateButtonHUD(hasUse);
    }
}
