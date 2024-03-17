using UnityEngine;

public class Activateable : MonoBehaviour
{
    protected bool active = false;

    public bool IsActive => active;

    public virtual void Activate()
    {
        if (!active)
        {
            active = true;
            gameObject.SetActive(true);
            OnActivate();
        }
        else
        {
            Debug.LogWarning("ITEM ALREADY ACTIVE");
        }
    }

    public virtual void Deactivate()
    {
        if (active)
        {
            active = false;
            gameObject.SetActive(false);
            OnDeactivate();
        }
        else
        {
            Debug.LogWarning("ITEM ALREADY INACTIVE");
        }
    }

    protected virtual void OnActivate()
    {

    }

    protected virtual void OnDeactivate()
    {

    }
}
