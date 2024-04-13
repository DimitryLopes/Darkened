using Zenject;
using System.Collections.Generic;
using System;

public class HUDManager
{
    private SignalBus signalBus;
    private HUD hud;
    private UIFactory uiFactory;

    private Dictionary<ItemType, UIItemView> instantiatedViews = new();

    [Inject]
    public HUDManager(HUD hud,SignalBus signalBus, UIFactory uiFactory)
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
        if (!instantiatedViews.ContainsKey(itemData.Item.Type)) return;

        UIItemView itemView = instantiatedViews[itemData.Item.Type];
        itemView.UpdateView();
    }

    public void CreateItemView(InventoryItemData data, Action<Item> onSelectCallback)
    {
        if (instantiatedViews.ContainsKey(data.Item.Type)) return;

        UIItemView itemView = uiFactory.CreateUIItemView(hud.ItemHUD.ItemViewContainer);
        instantiatedViews.Add(data.Item.Type, itemView);
        itemView.SetUp(data, onSelectCallback);
        return;
    }
}
