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

    public void CreateItemView(InventoryItemData itemData, Action<Item> onSelectCallback = null)
    {
        hud.ItemHUD.CreateItemView(itemData, onSelectCallback);
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
