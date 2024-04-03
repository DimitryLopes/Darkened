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

    private void Start()
    {
        PopulateMazeSizeContainer();
        PopulateObjectivesContainer();
        PopulateDifficultyContainer();
    }

    private void PopulateContainer<T>(List<T> selectables, UISelectableGroup customSelectableGroup, Func<T, IUISelectable> selector)
    {
        List<UISelectableItem> items = new List<UISelectableItem>();
        foreach (T selectableData in selectables)
        {
            IUISelectable uISelectable = selector(selectableData);
            UISelectableItem selectable = uiFactory.CreateUISelectableItem(customSelectableGroup.Container);
            selectable.SetUp(uISelectable);
            items.Add(selectable);
        }
        customSelectableGroup.Setup(items);
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
