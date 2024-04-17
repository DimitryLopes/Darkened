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

    public bool HasUseCallback => onSelectCallback != null;
    public ItemType ItemType => itemData.Item.Type;

    public bool IsSelected { get; private set; }

    public void UpdateView(InventoryItemData data, bool overrideCallback, Action<Item> onSelectCallback = null)
    {
        if (overrideCallback || !HasUseCallback)
        {
            this.onSelectCallback = onSelectCallback;
        }

        if (data.Amount <= 0)
        {
            Deactivate();
            return;
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

    public override void OnActivate()
    {
        itemImage.gameObject.SetActive(true);
    }

    public override void OnDeactivate()
    {
        itemImage.gameObject.SetActive(false);
        UpdateView(new InventoryItemData(), true);
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

    public void Deselect()
    {
        RawDeselect();
    }

    public void RawDeselect()
    {
        selectedOutline.gameObject.SetActive(false);
    }
}
