using Zenject;
using UnityEngine;

public class UIFactory
{
    private readonly DiContainer container;
    private readonly UISelectableItem uiSelectableItemPrefab;
    private readonly UIItemView uiItemViewPrefab;

    public UIFactory(DiContainer container, UISelectableItem uiSelectableItemPrefab, UIItemView uiItemViewPrefab)
    {
        this.uiSelectableItemPrefab = uiSelectableItemPrefab;
        this.uiItemViewPrefab = uiItemViewPrefab;
        this.container = container;
    }

    public UISelectableItem CreateUISelectableItem(Transform parent)
    {
        UISelectableItem selectable = container.InstantiatePrefabForComponent<UISelectableItem>(uiSelectableItemPrefab, parent);
        return selectable;
    }

    public UIItemView CreateUIItemView(Transform parent)
    {
        UIItemView view = container.InstantiatePrefabForComponent<UIItemView>(uiItemViewPrefab, parent);
        return view;
    }
}