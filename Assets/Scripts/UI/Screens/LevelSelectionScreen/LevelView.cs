using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelView : Activateable
{
    [SerializeField]
    private TextMeshProUGUI levelNameText;
    [SerializeField]
    private UIAnimationComponent hoverAnimation;
    [SerializeField]
    private UIAnimationComponent clickAnimation;
    [SerializeField]
    private Button button;

    public PresetLevelData LevelData { get; private set; }
    private Action<PresetLevelData> onViewClickedCallback;
    private void Start()
    {
        button.onClick.AddListener(OnButtonClick);
    }

    public void Setup(PresetLevelData data, Action<PresetLevelData> onViewClickedCallback)
    {
        levelNameText.text = $"{data.ID}";
        LevelData = data;
        this.onViewClickedCallback = onViewClickedCallback;
    }

    private void OnButtonClick()
    {
        clickAnimation.PlayInAnimations();
        onViewClickedCallback?.Invoke(LevelData);
    }
}
