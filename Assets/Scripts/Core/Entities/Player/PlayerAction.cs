using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    protected PlayerStatus status;

    public bool CanAct { get; protected set; }

    public void ToggleActing(bool value)
    {
        CanAct = value;
    }
}
