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

    public void UpdateItemView(InventoryItemData itemData)
    {
        hud.ItemHUD.UpdateItemView(itemData);
    }

    public void CreateItemView(InventoryItemData data, Action<Item> onSelectCallback = null)
    {
        hud.ItemHUD.CreateItemView(data, onSelectCallback);
    }

    public void UpdateBottomHUD(Item item, int index, int count)
    {
        hud.BottomHUD.UpdateButtonHUD(item, index, count);
    }
}
