using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILevelSelectionScreen : UIScreen<LevelSelectionScreenController>
{
    [SerializeField]
    private LevelView levelViewPrefab;
    [SerializeField]
    private GameObject levelViewContainer;
    [SerializeField]
    private Button backButton;

    private List<LevelView> levelViews = new List<LevelView>();
    private PresetLevelData selectedLevelData;

    private void Start()
    {
        backButton.onClick.AddListener(Hide);
    }

    protected override void OnBeforeShow()
    {
        base.OnBeforeShow();
        foreach (PresetLevelData levelData in Controller.LevelDatas)
        {
            LevelView levelView = GetAvailableLevelView();
            levelView.Setup(levelData, OnLevelViewClicked);
        }

    }

    protected override void OnAfterHide()
    {
        base.OnAfterHide();
        if (selectedLevelData != null)
        {
            Controller.OnLevelSelected?.Invoke(selectedLevelData);
            selectedLevelData = null;
        }
    }

    private void OnLevelViewClicked(PresetLevelData levelData)
    {
        selectedLevelData = levelData;
        Hide();
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
