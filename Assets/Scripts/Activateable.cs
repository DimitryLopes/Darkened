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
            gameObject.SetActive(true);
            OnActivate();
        }
        else
        {
            Debug.LogWarning("ITEM ALREADY ACTIVE");
        }
    }

    public void Deactivate()
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
