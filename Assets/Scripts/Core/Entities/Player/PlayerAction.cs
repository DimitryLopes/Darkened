using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    protected EntityStatus status;

    public bool CanAct { get; protected set; }

    public virtual void ToggleActing(bool value)
    {
        CanAct = value;
    }
}
