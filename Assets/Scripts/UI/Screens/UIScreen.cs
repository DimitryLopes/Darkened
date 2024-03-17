using UnityEngine;

public abstract class UIScreen<T> : MonoBehaviour, IScreen where T : ScreenController
{
    public T Controller { get; private set; }

    public bool IsShown { get; private set; }

    public virtual void Show<T>(T controller)
    {
        gameObject.SetActive(true);
        IsShown = true;
        OnBeforeShow();
        OnAfterShow();
    }

    public virtual void Hide()
    {
        OnBeforeHide();
        gameObject.SetActive(false);
        IsShown = false;
        OnAfterHide();
    }

    public virtual void OnBeforeShow() { }
    public virtual void OnAfterShow() { }
    public virtual void OnBeforeHide() { }
    public virtual void OnAfterHide() { }
}