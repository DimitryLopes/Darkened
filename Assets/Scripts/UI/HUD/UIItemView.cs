using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

public class UIItemView : Activateable, ISelectable
{
    [SerializeField]
    private Image itemImage;
    [SerializeField]
    private TextMeshProUGUI itemAmountText;
    [SerializeField]
    private Image selectedOutline;
    
    private InventoryItemData itemData;
    private Action<Item> onSelectCallback;
    private Action onDeselectCallback;
    private Action<UIItemView> onDeactivateCallback;
    private Action<UIItemView> onActivateCallback;

    public bool HasUseCallback => onSelectCallback != null;
    public ItemType ItemType => itemData.Item.Type;

    public bool IsSelected { get; private set; }

    public void UpdateView(InventoryItemData data, bool overrideCallback, Action<Item> onSelectCallback = null, Action onDeselectCallback = null)
    {
        if (data.Amount <= 0)
        {
            Deactivate();
            return;
        }

        if (overrideCallback)
        {
            this.onSelectCallback = onSelectCallback;
            this.onDeselectCallback = onDeselectCallback;
        }

        itemData = data;
        Activate();

        itemAmountText.gameObject.SetActive(itemData.Amount > 1);
        itemAmountText.text = itemData.Amount.ToString();

        itemImage.gameObject.SetActive(itemData.Item != null);
        itemImage.sprite = itemData.Item?.Icon;


        itemAmountText.gameObject.SetActive(data.Amount > 1);
        itemAmountText.text = data.Amount.ToString();
    }

    public override void Activate()
    {
        if (active) return;

        active = true;
        OnActivate();
    }

    public override void Deactivate()
    {
        if (!active) return;

        active = false;
        OnDeactivate();
    }

    public override void OnDeactivate()
    {
        itemImage.gameObject.SetActive(false);
        itemData = new InventoryItemData();
        onDeactivateCallback?.Invoke(this);
    }

    public override void OnActivate()
    {
        itemImage.gameObject.SetActive(true);
        onActivateCallback?.Invoke(this);
    }

    public void Select()
    {
        selectedOutline.gameObject.SetActive(true);
        OnSelect();
    }

    private void OnSelect()
    {
        onSelectCallback?.Invoke(itemData.Item);
    }

    private void OnDeselect()
    {
        onDeselectCallback?.Invoke();
    }

    public void Deselect()
    {
        RawDeselect();
        OnDeselect();
    }

    public void RawDeselect()
    {
        selectedOutline.gameObject.SetActive(false);
    }

    public void SetActivatableCallbacks(Action<UIItemView> onActivateCallback, Action<UIItemView> onDeactivateCallback)
    {
        this.onActivateCallback = onActivateCallback;
        this.onDeactivateCallback = onDeactivateCallback;
    }
}
