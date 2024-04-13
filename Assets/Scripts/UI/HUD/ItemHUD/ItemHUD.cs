using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ItemHUD : Activateable
{
    [Inject]
    private UIFactory uiFactory;
    [Inject]
    private SignalBus signalBus;

    [SerializeField]
    private Transform itemViewContainer;

    private UIItemView selectedItemView;

    private Dictionary<ItemType, UIItemView> instantiatedViews = new();
    private List<UIItemView> activeViews = new();
    public Transform ItemViewContainer => itemViewContainer;


    private void Start()
    {
        signalBus.Subscribe<OnInventoryItemSelectedSignal>(OnItemSelected);
        signalBus.Subscribe<OnBottomHUDNextButtonClickedSignal>(OnItemSelected);
        signalBus.Subscribe<OnBottomHUDPreviousButtonClickedSignal>(OnItemSelected);
        signalBus.Subscribe<OnBottomHUDUseButtonClickedSignal>(OnItemSelected);
    }

    private void OnItemSelected(OnInventoryItemSelectedSignal signal)
    {
        if(selectedItemView != null)
        {
            selectedItemView.Deselect();
        }

        selectedItemView = instantiatedViews[signal.Item.Type];
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
    }

    public void CreateItemView(InventoryItemData data, Action<Item> onSelectCallback)
    {
        if (instantiatedViews.ContainsKey(data.Item.Type)) return;

        UIItemView itemView = uiFactory.CreateUIItemView(itemViewContainer);
        instantiatedViews.Add(data.Item.Type, itemView);
        itemView.SetActivatableCallbacks(OnItemViewActivated, OnItemViewDeactivated);
        itemView.SetUp(data, onSelectCallback);
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
}
