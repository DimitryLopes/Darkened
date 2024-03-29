using UnityEngine;

public class Activateable : MonoBehaviour, IActivateable
{
    protected bool active = false;

    public bool IsActive => active;

    public virtual void Activate()
    {
        if (active) return;

        active = true;
        gameObject.SetActive(true);
        OnActivate();
    }

    public virtual void Deactivate()
    {
        if (!active) return;

        active = false;
        gameObject.SetActive(false);
        OnDeactivate();
    }

    public virtual void OnActivate()
    {

    }

    public virtual void OnDeactivate()
    {

    }
}
