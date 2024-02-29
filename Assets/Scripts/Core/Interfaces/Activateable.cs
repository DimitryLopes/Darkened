using UnityEngine;

public class Activateable : MonoBehaviour
{
    private bool active;

    public bool IsActive => active;

    public void Activate()
    {
        if (!active)
        {
            active = true;
            OnActivate();
        }
    }

    public void Dectivate()
    {
        if (active)
        {
            active = false;
            OnDeactivate();
        }
    }

    protected virtual void OnActivate()
    {

    }

    protected virtual void OnDeactivate()
    {

    }
}
