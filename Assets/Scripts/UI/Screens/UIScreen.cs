using UnityEngine;
using Zenject;

public abstract class UIScreen<U> : MonoBehaviour, IScreen where U : ScreenController
{
    [Inject]
    private SignalBus signalBus;

    public U Controller { get; private set; }

    public bool IsShown { get; private set; }

    public virtual void Show<T>(T controller) where T : ScreenController
    {
        Controller = controller as U;
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

    protected virtual void OnBeforeShow() 
    {
        signalBus.Fire(new OnScreenBeforeShowSignal(this));
    }
    protected virtual void OnAfterShow() 
    {
        signalBus.Fire(new OnScreenAfterShowSignal(this));
    }
    protected virtual void OnBeforeHide() 
    {
        signalBus.Fire(new OnScreenBeforeHideSignal(this));
    }
    protected virtual void OnAfterHide() 
    {
        signalBus.Fire(new OnScreenAfterHideSignal(this));
    }
}