using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class UICustomLevelSelection : MonoBehaviour
{
    [Inject]
    private MazeSizeDataBase mazeSizeDataBase;
    [Inject]
    private ObjectivesDataBase objectivesDataBase;
    [Inject]
    private DifficultyDataBase difficultyDataBase;
    [Inject]
    private UIFactory uiFactory;

    [SerializeField]
    private UISelectableGroup customSizeSelectableGroup;
    [SerializeField]
    private UISelectableGroup customObjectiveSelectableGroup;
    [SerializeField]
    private UISelectableGroup customDifficultySelectableGroup;

    private List<UISelectableItem> items = new List<UISelectableItem>();

    private void OnEnable()
    {
        PopulateMazeSizeContainer();
        PopulateObjectivesContainer();
        PopulateDifficultyContainer();
    }

    private void OnDisable()
    {
        customSizeSelectableGroup.Deactivate();
        customObjectiveSelectableGroup.Deactivate();
        customDifficultySelectableGroup.Deactivate();
    }

    private void PopulateContainer<T>(List<T> selectables, UISelectableGroup customSelectableGroup, Func<T, IUISelectable> selector)
    {
        List<UISelectableItem> items = new List<UISelectableItem>();
        foreach (T selectableData in selectables)
        {
            IUISelectable uiSelectable = selector(selectableData);
            items.Add(GetAvailableSelectable(customSelectableGroup, uiSelectable));
        }
        customSelectableGroup.Setup(items);
        customSelectableGroup.Activate();
    }

    private UISelectableItem GetAvailableSelectable(UISelectableGroup group, IUISelectable uiSelectable)
    {
        foreach (UISelectableItem item in items)
        {
            if (item.IsActive) continue;
            item.SetUp(uiSelectable, SelectableItemSize.Small);
            item.Activate();
            return item;
        }

        UISelectableItem selectable = uiFactory.CreateUISelectableItem(group.Container);
        selectable.SetUp(uiSelectable, SelectableItemSize.Small);
        selectable.Activate();
        items.Add(selectable);
        return selectable;
    }

    private void PopulateObjectivesContainer()
    {
        List<ObjectiveData> selectables = objectivesDataBase.GetAllObjectives();
        PopulateContainer(selectables, customObjectiveSelectableGroup, (data) => data);
    }

    private void PopulateMazeSizeContainer()
    {
        List<MazeSizeData> selectables = mazeSizeDataBase.GetSizeDatas();
        PopulateContainer(selectables, customSizeSelectableGroup, (data) => data);
    }

    private void PopulateDifficultyContainer()
    {
        List<DifficultyData> selectables = difficultyDataBase.DifficultyList;
        PopulateContainer(selectables, customDifficultySelectableGroup, (data) => data);
    }
}
