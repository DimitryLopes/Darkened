using UnityEngine;
using UnityEngine.Events;

public class BaseStateData
{
    public BaseStateData(IStateUser user, bool isSprinting,
        UnityAction onDeactivateCallback, UnityAction onActivateCallback, UnityAction onStateCompletedCallback)
    {
        User = user;
        IsSprinting = isSprinting;
        OnDeactivateCallback = onDeactivateCallback;
        OnActivateCallback = onActivateCallback;
        OnStateCompletedCallback = onStateCompletedCallback;
    }

    public IStateUser User { get; private set; }
    public bool IsSprinting { get; private set; }
    public UnityAction OnDeactivateCallback { get; private set; }
    public UnityAction OnActivateCallback { get; private set; }
    public UnityAction OnStateCompletedCallback { get; private set; }
}
