using UnityEngine;

[CreateAssetMenu(fileName = "Rename Me", menuName = "Scriptable Objects/Datas/Unlock Condition Data")]
public class UnlockConditionData : ScriptableObject
{
    [SerializeField]
    private UnlockCondition unlockCondition;

    public UnlockCondition UnlockCondition => unlockCondition;
}
