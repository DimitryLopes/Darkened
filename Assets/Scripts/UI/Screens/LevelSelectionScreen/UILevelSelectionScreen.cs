using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILevelSelectionScreen : UIScreen<LevelSelectionScreenController>
{
    [SerializeField]
    private LevelView levelViewPrefab;
    [SerializeField]
    private GameObject levelViewContainer;

    private List<LevelView> levelViews = new List<LevelView>();
    private System.Action<LevelData> OnLevelViewClickCallback;

    protected override void OnBeforeShow()
    {
        base.OnBeforeShow();
        foreach (PresetLevelData levelData in Controller.LevelDatas)
        {
            LevelView levelView = GetAvailableLevelView();
            levelView.Setup(levelData, OnLevelViewClicked);
        }

    }

    private void OnLevelViewClicked(PresetLevelData levelData)
    {
        Controller.OnLevelSelected?.Invoke(levelData);
    }

    private LevelView GetAvailableLevelView()
    {
        foreach(LevelView levelView in levelViews)
        {
            if (!levelView.IsActive)
            {
                levelView.Activate();
                return levelView;
            }
        }

        LevelView newLevelView = Instantiate(levelViewPrefab, levelViewContainer.transform);
        levelViews.Add(newLevelView);
        newLevelView.Activate();
        return newLevelView;
    }
}
