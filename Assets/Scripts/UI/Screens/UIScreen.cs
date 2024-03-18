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

    protected virtual void OnBeforeShow() { }
    protected virtual void OnAfterShow() { }
    protected virtual void OnBeforeHide() { }
    protected virtual void OnAfterHide() { }
}