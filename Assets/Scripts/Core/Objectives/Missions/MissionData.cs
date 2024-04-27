using UnityEngine;

public abstract class MissionData : ScriptableObject
{
    [SerializeField]
    private string description;

    public abstract int GetTargetProgress(DifficultyType difficulty);

    public string Description => description;
}
