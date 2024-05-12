using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    private Action<UIItemView> onSelectCallback;

    public bool HasUseCallback => itemData.Item is IUsable;

    public Item Item => itemData.Item;
    public bool HasItem => itemData.Item != null;
    public int Index { get; private set; }
    public bool IsSelected { get; private set; }

    void OnEnable()
    {
        selectButton.onClick.AddListener(Select);
    }

    void OnDisable()
    {
        selectButton.onClick.RemoveListener(Select);
    }

    public void CreateView(InventoryItemData data, Action<UIItemView> onSelectCallback, int index)
    {
        Index = index;
        itemData = data;
        this.onSelectCallback = onSelectCallback;
    }

    public void UpdateView(InventoryItemData data)
    {
        itemData = data;
        if (data.Amount <= 0)
        {
            Deactivate();
            return;
        }

        Activate();

        itemAmountText.gameObject.SetActive(itemData.Amount > 1);
        itemAmountText.text = itemData.Amount.ToString();

        itemImage.gameObject.SetActive(itemData.Item != null);
        itemImage.sprite = itemData.Item?.Icon;


        itemAmountText.gameObject.SetActive(data.Amount > 1);
        itemAmountText.text = data.Amount.ToString();
    }

    public override void Activate(bool forced = false)
    {
        if (active && !forced) return;

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
        UpdateView(new InventoryItemData());
    }

    public void Select()
    {
        selectedOutline.gameObject.SetActive(true);
        OnSelect();
    }

    private void OnSelect()
    {
        onSelectCallback?.Invoke(this);
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
