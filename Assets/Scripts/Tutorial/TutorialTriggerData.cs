using System;
using UnityEngine;

[Serializable]
public struct TutorialTriggerData
{
    [SerializeField]
    private TutorialTriggerType triggerType;
    [SerializeField, ShowIf("triggerType", TutorialTriggerType.Level)]
    private int levelID;
    [SerializeField, ShowIf("triggerType", TutorialTriggerType.Item, TutorialTriggerType.Interaction)]
    private ItemType itemType;

    public TutorialTriggerType TriggerType => triggerType;
    public int LevelID => levelID;
    public ItemType ItemType => itemType;
}

public enum TutorialTriggerType
{
    Level,
    Interaction,
    Item,
}
