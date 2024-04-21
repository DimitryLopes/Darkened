using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnlockConditionDataBase", menuName = "Scriptable Objects/Data Bases/Unlock Condition Data Base")]
public class UnlockConditionDataBase : ScriptableObject
{
    [SerializeField]
    private List<UnlockConditionData> unlockConditions;

    public List<UnlockConditionData> UnlockConditions => unlockConditions;
}
