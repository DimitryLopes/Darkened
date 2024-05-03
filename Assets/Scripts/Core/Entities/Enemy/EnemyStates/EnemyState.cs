using UnityEngine.Events;

public abstract class EnemyState<U> : IEnemyState where U : BaseEnemyStateData
{
    protected Enemy Enemy => Data.Enemy;
    public bool IsActive { get; private set; }
    public bool IsCompleted { get; private set; }
    protected U Data { get; private set; }
    public UnityAction OnStateEndedCallback { get; set; }
    public UnityAction OnStateStartedCallback { get; set; }


    public EnemyState(U data)
    {
        Data = data;
    }

    public abstract void HandleState();

    public void Activate()
    {
        if (!IsActive)
        {
            IsActive = true;
            IsCompleted = false;
            OnActivate();
        }
    }

    public void Deactivate()
    {
        if (IsActive)
        {
            IsActive = false;
            IsCompleted = true;
            OnDeactivate();
        }
    }

    public void Complete()
    {
        if (!IsCompleted)
        {
            IsCompleted = true;
            OnComplete();
            Deactivate();
        }
    }

    public void RawDeactivate()
    {
        IsActive = false;
        IsCompleted = true;
    }

    public virtual void OnDeactivate()
    {
        Data.OnDeactivateCallback?.Invoke();
    }

    public virtual void OnActivate() 
    {
        Data.OnActivateCallback?.Invoke();
        OnStateStartedCallback?.Invoke();
        OnStateStartedCallback = null;
    }

    public virtual void OnComplete()
    {
        Data.OnStateCompletedCallback?.Invoke();
        OnStateEndedCallback?.Invoke();
        OnStateEndedCallback = null;
    }
}
