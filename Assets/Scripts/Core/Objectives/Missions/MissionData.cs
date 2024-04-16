using UnityEngine;

public class MissionData : ScriptableObject
{
    [SerializeField]
    private string description;

    public virtual int ProgressTarget { get; }
    public string Description => description;
}
