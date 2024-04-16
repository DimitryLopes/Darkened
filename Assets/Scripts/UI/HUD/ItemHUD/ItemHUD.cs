using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ItemHUD : Activateable
{
    [Inject]
    private UIFactory uiFactory;
    [Inject]
    private SignalBus signalBus;
    [Inject]
    private HUDManager hudManager;

    [SerializeField]
    private Transform itemViewContainer;

    private UIItemView selectedItemView;
    private int selectionIndex;
    private Dictionary<ItemType, UIItemView> instantiatedViews = new();
    private List<UIItemView> activeViews = new();
    public Transform ItemViewContainer => itemViewContainer;


    private void Start()
    {
        selectionIndex = 0;
        signalBus.Subscribe<OnBottomHUDNextButtonClickedSignal>(OnNextButtonClicked);
        signalBus.Subscribe<OnBottomHUDPreviousButtonClickedSignal>(OnPreviousButtonClicked);
    }

    public void Clear()
    {
        foreach(UIItemView view in instantiatedViews.Values)
        {
            view.Deactivate();
        }
    }

    public void UpdateItemView(InventoryItemData data)
    {
        if (!instantiatedViews.ContainsKey(data.Item.Type)) return;

        UIItemView itemView = instantiatedViews[data.Item.Type];
        itemView.UpdateView(data);
        OnViewsChanged();
    }

    public void CreateItemView(InventoryItemData data, Action<Item> onSelectCallback)
    {
        if (instantiatedViews.ContainsKey(data.Item.Type)) return;

        UIItemView itemView = uiFactory.CreateUIItemView(itemViewContainer);
        instantiatedViews.Add(data.Item.Type, itemView);
        itemView.SetActivatableCallbacks(OnItemViewActivated, OnItemViewDeactivated);
        itemView.SetUp(data, onSelectCallback);
        itemView.Activate();

        if(selectedItemView == null && data.Item is IUsable)
        {
            selectionIndex = activeViews.Count - 1;
        }
        OnViewsChanged();
        return;
    }

    private void OnItemViewActivated(UIItemView view)
    {
        activeViews.Add(view);
    }

    private void OnItemViewDeactivated(UIItemView view)
    {
        activeViews.Remove(view);
    }

    private void OnNextButtonClicked()
    {
        selectionIndex++;
        if(selectionIndex > activeViews.Count -1)
        {
            selectionIndex = 0;
        }
        OnViewsChanged();
    }

    private void OnPreviousButtonClicked()
    {
        selectionIndex--;
        if (selectionIndex < 0)
        {
            selectionIndex = activeViews.Count - 1;
        }
        OnViewsChanged();
    }

    private void OnViewsChanged()
    {
        if(selectedItemView != null)
        {
            selectedItemView.Deselect();
        }

        activeViews[selectionIndex].Select();
        selectedItemView = activeViews[selectionIndex];
        hudManager.UpdateBottomHUD(activeViews[selectionIndex].HasUseCallback, activeViews.Count);
    }

}
