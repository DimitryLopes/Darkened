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
    }

    public void Deactivate()
    {
        if (active)
        {
            active = false;
            gameObject.SetActive(false);
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
