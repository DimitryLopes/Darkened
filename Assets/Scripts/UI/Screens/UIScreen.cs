using UnityEngine;
using Zenject;

public class UIScreen<U> : MonoBehaviour, IScreen where U : ScreenController
{
    [Inject]
    private SignalBus signalBus;

    [SerializeField]
    private CanvasGroup canvasGroup; 
    [SerializeField]
    private UIAnimationComponent screenAnimation;

    public U Controller { get; private set; }
    public bool IsShown { get; private set; }
    public bool IsHiding { get; protected set; } = false;
    protected bool IsFirstShow { get; private set; } = true;


    public virtual void Show<T>(T controller) where T : ScreenController
    {
        transform.SetAsLastSibling();
        canvasGroup.interactable = true;
        Controller = controller as U;
        gameObject.SetActive(true);

        OnBeforeShow();
        screenAnimation.PlayInAnimations(OnAfterShow);
    }
    public void Show()
    {
        if (Controller != null)
            Show(Controller);
        else
        {
            Debug.LogError("Error: Screen had no controller when showing it");
        }
    }

    public virtual void Hide()
    {
        if (IsHiding || !IsShown) return;

        OnBeforeHide();
        IsHiding = true;
        canvasGroup.interactable = false;
        screenAnimation.PlayOutAnimations(OnHideAnimationFinish);
    }

    protected void OnHideAnimationFinish()
    {
        IsHiding = false;
        gameObject.SetActive(false);
        OnAfterHide();
    }

    protected virtual void OnBeforeShow() 
    {
        signalBus.Fire(new OnScreenBeforeShowSignal(this));
    }
    protected virtual void OnAfterShow()
    {
        IsShown = true;
        IsFirstShow = false;
        signalBus.Fire(new OnScreenAfterShowSignal(this));
    }
    protected virtual void OnBeforeHide() 
    {
        signalBus.Fire(new OnScreenBeforeHideSignal(this));
    }
    protected virtual void OnAfterHide()
    {
        IsShown = false;
        signalBus.Fire(new OnScreenAfterHideSignal(this));
    }

    public void HideImmediately(bool sendEvent)
    {
        canvasGroup.interactable = false;
        IsHiding = false;
        gameObject.SetActive(false);
        if (!sendEvent) return;
        OnBeforeHide();
        OnAfterHide();
    }
}