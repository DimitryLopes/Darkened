using UnityEngine;

public struct TutorialTriggerData
{
    [SerializeField]
    private TutorialTriggerType TriggerType;
    [SerializeField, ShowIf("TriggerType", TutorialTriggerType.Level)]
    private int LevelID;
    [SerializeField, ShowIf("TriggerType", TutorialTriggerType.Item, TutorialTriggerType.Interaction)]
    private ItemType ItemType;
}

public enum TutorialTriggerType
{
    Level,
    Interaction,
    Item,
}
