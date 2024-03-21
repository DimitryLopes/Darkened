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
    private UIFactory uiFactory;

    [SerializeField]
    private UISelectableGroup customSizeSelectableGroup;
    [SerializeField]
    private UISelectableGroup customObjectiveSelectableGroup;

    private void Start()
    {
        PopulateMazeSizeContainer();
        PopulateObjectivesContainer();
    }

    private void PopulateObjectivesContainer()
    {
        List<Objective> selectables = objectivesDataBase.GetAllObjectives();
        List<UISelectableItem> items = new List<UISelectableItem>();
        foreach (IUISelectable uISelectable in selectables)
        {
            UISelectableItem selectable = uiFactory.CreateUISelectableItem(customObjectiveSelectableGroup.Container);
            selectable.SetUp(uISelectable);
            items.Add(selectable);
        }
        customObjectiveSelectableGroup.Setup(items);
    }

    private void PopulateMazeSizeContainer()
    {
        List<MazeSizeData> selectables = mazeSizeDataBase.GetSizeDatas();
        List<UISelectableItem> items = new List<UISelectableItem>();
        foreach (IUISelectable uISelectable in selectables)
        {
            UISelectableItem selectable = uiFactory.CreateUISelectableItem(customSizeSelectableGroup.Container);
            selectable.SetUp(uISelectable);
            items.Add(selectable);
        }
        customSizeSelectableGroup.Setup(items);
    }
}
