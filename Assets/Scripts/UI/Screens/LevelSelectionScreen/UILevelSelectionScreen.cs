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
    private GameObject nextLevelsViewContainer;
    [SerializeField]
    private GameObject previousLevelsViewContainer;
    [SerializeField]
    private Button backButton;
    [SerializeField]
    private Button nextPageButton;
    [SerializeField]
    private Button previousPageButton;
    [SerializeField]
    private UIAnimationComponent pageChangeAnimation;
    [SerializeField]
    private UIAnimationComponent instantChangeAnimation;

    private List<LevelView> levelViews = new List<LevelView>();
    private List<LevelView> previouslevelViews = new List<LevelView>();
    private List<LevelView> nextlevelViews = new List<LevelView>();
    private PresetLevelData selectedLevelData;
    private int currentPageStartingLevelIndex = 0;

    private void Start()
    {
        backButton.onClick.AddListener(Hide);
        nextPageButton.onClick.AddListener(PlayNextPageAnimation);
        previousPageButton.onClick.AddListener(PlayPreviousPageAnimation);
    }

    protected override void OnBeforeShow()
    {
        base.OnBeforeShow();
        instantChangeAnimation.PlayInAnimations();
        currentPageStartingLevelIndex = 0;
        foreach (PresetLevelData levelData in Controller.LevelDatas)
        {
            LevelView levelView = GetAvailableLevelView();
            levelView.Setup(levelData, OnLevelViewClicked);
        }
        PopulatePages();
    }


    private void PlayNextPageAnimation()
    {
        foreach (LevelView levelView in previouslevelViews)
        {
            levelView.Deactivate();
        }
        currentPageStartingLevelIndex += Controller.LevelsPerPage;
        currentPageStartingLevelIndex = currentPageStartingLevelIndex > Controller.LevelDatas.Count - 1 ? 0 : currentPageStartingLevelIndex;
        pageChangeAnimation.PlayInAnimations(OnNextPageAnimationFinish);
    }

    private void PlayPreviousPageAnimation()
    {
        foreach (LevelView levelView in nextlevelViews)
        {
            levelView.Deactivate();
        }

        currentPageStartingLevelIndex -= Controller.LevelsPerPage;
        currentPageStartingLevelIndex = currentPageStartingLevelIndex < 0 ? currentPageStartingLevelIndex : Controller.LevelDatas.Count - 1;
        pageChangeAnimation.PlayOutAnimations(OnPreviousPageAnimationFinish);
    }

    private void OnNextPageAnimationFinish()
    {
        PopulatePages();
        instantChangeAnimation.PlayInAnimations();
    }
    private void OnPreviousPageAnimationFinish()
    {
        PopulatePages();
        instantChangeAnimation.PlayOutAnimations();
    }

    private void PopulatePages()
    {
        DeactivateAllLevels();
        currentPageStartingLevelIndex = Mathf.Clamp(currentPageStartingLevelIndex, 0, Controller.LevelDatas.Count - 1);
        int levelsPerPage = Controller.LevelsPerPage;
        int totalLevels = levelViews.Count;
        int totalPages = Mathf.CeilToInt((float)totalLevels / levelsPerPage);

        int currentPageIndex = currentPageStartingLevelIndex / levelsPerPage;

        int[] pagesToShow = new int[]
        {
        WrapPage(currentPageIndex - 1), 
        currentPageIndex,               
        WrapPage(currentPageIndex + 1)  
        };

        for (int pageOffset = 0; pageOffset < pagesToShow.Length; pageOffset++)
        {
            int pageIndex = pagesToShow[pageOffset];
            Transform container = pageOffset switch
            {
                0 => previousLevelsViewContainer.transform,
                1 => levelViewContainer.transform,
                2 => nextLevelsViewContainer.transform,
                _ => null,
            };

            int startLevel = pageIndex * levelsPerPage;
            int endLevel = Mathf.Min(startLevel + levelsPerPage, totalLevels);

            for (int i = startLevel; i < endLevel; i++)
            {
                levelViews[i].transform.SetParent(container);
                levelViews[i].Activate();
            }
        }
        int WrapPage(int page) { return (page + totalPages) % totalPages; };
    }


    protected override void OnAfterHide()
    {
        base.OnAfterHide();
        DeactivateAllLevels();
        if (selectedLevelData != null)
        {
            Controller.OnLevelSelected?.Invoke(selectedLevelData);
            selectedLevelData = null;
        }
    }

    private void DeactivateAllLevels()
    {
        foreach (LevelView levelView in levelViews)
        {
            levelView.Deactivate();
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
