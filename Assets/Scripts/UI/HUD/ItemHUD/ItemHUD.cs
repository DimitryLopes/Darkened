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
    [SerializeField]
    private ItemHUDKeyboardSelector keyboardSelector;

    private UIItemView selectedItemView;
    private int selectionIndex;
    private List<UIItemView> instantiatedViews = new();

    private void Start()
    {
        selectionIndex = 0;
        signalBus.Subscribe<OnBottomHUDNextButtonClickedSignal>(OnNextButtonClicked);
        signalBus.Subscribe<OnBottomHUDPreviousButtonClickedSignal>(OnPreviousButtonClicked);
        signalBus.Subscribe<OnPlayerTryToUseItemSignal>(OnPlayerTryToUseItem);
        keyboardSelector.gameObject.SetActive(!GameManager.IsOnPhone);
    }

    private void OnPlayerTryToUseItem()
    {
        if (selectedItemView == null || !selectedItemView.HasUseCallback) return;

        signalBus.Fire(new OnPlayerUsedItemSignal(selectedItemView.Item as UsableItem));
    }

    public override void OnActivate()
    {
        if (instantiatedViews.Count > 0)
            instantiatedViews[0].Select();

        base.OnActivate();
    }

    public void Clear()
    {
        foreach(UIItemView view in instantiatedViews)
        {
            view.Deactivate();
        }
    }

    public void CreateItemView(InventoryItemData data)
    {
        UIItemView itemView = GetAvailableItemView();
        itemView.UpdateView(data);
        if(selectedItemView == itemView)
        {
            selectedItemView.Select();
        }
    }

    public void UpdateItemView(InventoryItemData data)
    {
        UIItemView view = GetView(data);
        if (view == null)
        {
            view = GetAvailableItemView();
        }
        if (view == null) return;

        view.UpdateView(data);
        return;
    }

    public UIItemView GetAvailableItemView()
    {
        foreach (UIItemView view in instantiatedViews) 
        {
            if (view.IsActive) continue;
                view.Activate();
            return view;
        }

        return null;
    }

    private UIItemView GetView(InventoryItemData data)
    {
        foreach(UIItemView view in instantiatedViews)
        {
            if(view.IsActive && view.Item == data.Item)
            {
                return view;
            }
        }
        return null;
    }

    public void CreateRawViews(int inventorySlots)
    {
        for(int i = 0; i < inventorySlots; i++)
        {
            UIItemView itemView = uiFactory.CreateUIItemView(itemViewContainer);
            itemView.CreateView(new InventoryItemData(), OnViewSelectClickCallback, i);
            instantiatedViews.Add(itemView);
        }
        keyboardSelector.SetViews(instantiatedViews);
        selectionIndex = 0;
    }

    private void OnNextButtonClicked()
    {
        selectionIndex++;
        if(selectionIndex > instantiatedViews.Count -1)
        {
            selectionIndex = 0;
        }
        instantiatedViews[selectionIndex].Select();
    }

    private void OnPreviousButtonClicked()
    {
        selectionIndex--;
        if (selectionIndex < 0)
        {
            selectionIndex = instantiatedViews.Count - 1;
        }
        instantiatedViews[selectionIndex].Select();
    }

    private void OnViewSelectClickCallback(UIItemView view)
    {
        bool canDeselect = view != selectedItemView && selectedItemView != null;
        if (canDeselect)
        {
            selectedItemView.Deselect();
        }

        selectionIndex = view.Index;
        selectedItemView = instantiatedViews[selectionIndex];
        hudManager.UpdateBottomHUD(instantiatedViews[selectionIndex].HasUseCallback);
    }
}
