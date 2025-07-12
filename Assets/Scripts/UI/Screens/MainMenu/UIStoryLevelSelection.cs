using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class UIStoryLevelSelection : MonoBehaviour
{
    [Inject]
    private LevelManager levelManager;
    [Inject]
    private UIFactory uiFactory;

    [SerializeField]
    private UISelectableGroup storyGroup;

    private List<UISelectableItem> items = new List<UISelectableItem>();

    private void OnEnable()
    {
        PopulateLevelContainer();
    }

    private void OnDisable()
    {
        storyGroup.Deactivate();
    }

    private void PopulateContainer<T>(List<T> selectables, Func<T, IUISelectable> selector)
    {
        foreach (T selectableData in selectables)
        {
            IUISelectable uiSelectable = selector(selectableData);
            GetAvailableSelectable(storyGroup, uiSelectable);
        }
        storyGroup.Setup(items);
        storyGroup.Activate();
    }

    private void PopulateLevelContainer()
    {
        List<RandomLevelData> selectables = levelManager.GetUnlockedLevels();
        PopulateContainer(selectables, (data) => data);
    }

    private UISelectableItem GetAvailableSelectable(UISelectableGroup group, IUISelectable uiSelectable)
    {
        foreach (UISelectableItem item in items)
        {
            if (item.IsActive) continue;
            item.SetUp(uiSelectable, SelectableItemSize.Big);
            item.Activate();
            return item;
        }

        UISelectableItem selectable = uiFactory.CreateUISelectableItem(group.Container);
        selectable.SetUp(uiSelectable, SelectableItemSize.Big);
        selectable.Activate();
        items.Add(selectable);
        return selectable;
    }
}
