using Zenject;
using System;

public class HUDManager
{
    private HUD hud;
    private SignalBus signalBus;
    private UIFactory uiFactory;


    [Inject]
    public HUDManager(HUD hud, SignalBus signalBus, UIFactory uiFactory)
    {
        this.hud = hud;
        this.signalBus = signalBus;
        this.uiFactory = uiFactory;
    }

    public void ShowHud()
    {
        hud.Show();
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

    public void UpdateBottomHUD(bool hasUse, int count)
    {
        hud.BottomHUD.UpdateButtonHUD(hasUse, count);
    }
}
