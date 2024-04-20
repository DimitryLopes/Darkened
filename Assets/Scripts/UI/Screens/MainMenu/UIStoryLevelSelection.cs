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

    private void OnEnable()
    {
        PopulateLevelContainer();
    }

    private void PopulateContainer<T>(List<T> selectables, UISelectableGroup customSelectableGroup, Func<T, IUISelectable> selector)
    {
        List<UISelectableItem> items = new List<UISelectableItem>();
        foreach (T selectableData in selectables)
        {
            IUISelectable uISelectable = selector(selectableData);
            UISelectableItem selectable = uiFactory.CreateUISelectableItem(customSelectableGroup.Container);
            selectable.SetUp(uISelectable, SelectableItemSize.Big);
            items.Add(selectable);
        }
        customSelectableGroup.Setup(items);
    }

    private void PopulateLevelContainer()
    {
        List<LevelData> selectables = levelManager.GetUnlockedLevels();
        PopulateContainer(selectables, storyGroup, (data) => data);
    }
}
