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
    [SerializeField]
    private Button selectButton;
    
    private InventoryItemData itemData;
    private Action<Item> onSelectCallback;
    private Action onDeselectCallback;

    public bool IsSelected { get; private set; }

    public void SetUp(InventoryItemData itemData, Action<Item> onSelectCallback = null, Action onDeselectCallback = null)
    {
        itemImage.sprite = itemData.Item.Icon;
        this.itemData = itemData;
        this.onSelectCallback = onSelectCallback;
        this.onDeselectCallback = onDeselectCallback;
        RawDeselect();
        SetOnButtonClickCallback(Select);
    }

    public void UpdateView()
    {
        itemAmountText.text = itemData.Amount.ToString();
    }

    public void Select()
    {
        selectedOutline.gameObject.SetActive(true);
        OnSelect();
    }

    private void OnSelect()
    {
        SetOnButtonClickCallback(Deselect);
        onSelectCallback?.Invoke(item);
    }

    private void OnDeselect()
    {
        SetOnButtonClickCallback(Select);
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

    private void SetOnButtonClickCallback(UnityAction callback)
    {
        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(callback);

    }
}
