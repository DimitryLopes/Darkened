using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player Status", menuName = "Scriptable Objects/Player Status")]
public class EntityStatusSO : ScriptableObject
{
    [SerializeField]
    private Status[] statuses;

    public IReadOnlyList<Status> Statuses => statuses;

}

[Serializable]
public struct Status
{
    [SerializeField]
    private StatusKey key;
    [SerializeField]
    private float value;

    public StatusKey Key => key;
    public float Value => value;
}
