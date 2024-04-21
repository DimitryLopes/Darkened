using System.Collections.Generic;
using UnityEngine;

public class UnlockConditionDataBase : ScriptableObject
{
    [SerializeField]
    private List<UnlockConditionData> unlockConditions;

    public List<UnlockConditionData> UnlockConditions => unlockConditions;
}
