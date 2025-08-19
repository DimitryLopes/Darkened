using UnityEngine;
using UnityEngine.Events;

public class WaitingStateData : BaseStateData
{
    public float WaitingTime { get; private set; }
    public WaitingStateData(IStateUser user, bool isSprinting, float waitingTime,
        UnityAction onDeactivateCallback, UnityAction onActivateCallback, UnityAction onStateCompletedCallback) : base(user, isSprinting, onDeactivateCallback, onActivateCallback, onStateCompletedCallback)
    {
        WaitingTime = waitingTime;
    }
}
