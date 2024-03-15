using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    protected PlayerStatus status;

    public bool CanAct { get; protected set; }

    public virtual void SetUp(PlayerStatus status)
    {
        this.status = status;
    }

    public void ToggleActing(bool value)
    {
        CanAct = value;
    }
}
