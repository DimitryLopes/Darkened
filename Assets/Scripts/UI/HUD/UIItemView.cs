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
    private Action<UIItemView> onDeactivateCallback;
    private Action<UIItemView> onActivateCallback;

    public bool HasUseCallback => onSelectCallback != null;

    public bool IsSelected { get; private set; }

    public void SetUp(InventoryItemData itemData, Action<Item> onSelectCallback = null, Action onDeselectCallback = null)
    {
        itemImage.sprite = itemData.Item.Icon;
        this.itemData = itemData;
        selectButton.interactable = false; //onSelectCallback != null;
        this.onSelectCallback = onSelectCallback;
        this.onDeselectCallback = onDeselectCallback;
        RawDeselect();

        itemAmountText.gameObject.SetActive(itemData.Amount > 1);
        itemAmountText.text = itemData.Amount.ToString();
        //SetOnButtonClickCallback(Select);
    }

    public void UpdateView(InventoryItemData data)
    {
        if(data.Amount <= 0)
        {
            Deactivate();
            return;
        }
        else
        {
            Activate();
        }
        
        itemAmountText.gameObject.SetActive(data.Amount > 1);
        itemAmountText.text = data.Amount.ToString();
    }

    public override void OnDeactivate()
    {
        onDeactivateCallback?.Invoke(this);
    }

    public override void OnActivate()
    {
        onActivateCallback?.Invoke(this);
    }

    public void Select()
    {
        selectedOutline.gameObject.SetActive(true);
        OnSelect();
    }

    private void OnSelect()
    {
        //SetOnButtonClickCallback(Deselect);
        onSelectCallback?.Invoke(itemData.Item);
    }

    private void OnDeselect()
    {
        //SetOnButtonClickCallback(Select);
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

    public void SetActivatableCallbacks(Action<UIItemView> onActivateCallback, Action<UIItemView> onDeactivateCallback)
    {
        this.onActivateCallback = onActivateCallback;
        this.onDeactivateCallback = onDeactivateCallback;
    }
}
