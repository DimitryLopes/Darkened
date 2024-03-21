using Zenject;
using UnityEngine;

public class UIFactory
{
    private readonly DiContainer container;
    private readonly UISelectableItem uiSelectableItemPrefab;

    public UIFactory(DiContainer container, UISelectableItem uiSelectableItemPrefab)
    {
        this.uiSelectableItemPrefab = uiSelectableItemPrefab;
        this.container = container;
    }

    public UISelectableItem CreateUISelectableItem(Transform parent)
    {
        UISelectableItem selectable = container.InstantiatePrefabForComponent<UISelectableItem>(uiSelectableItemPrefab, parent);
        return selectable;
    }
}