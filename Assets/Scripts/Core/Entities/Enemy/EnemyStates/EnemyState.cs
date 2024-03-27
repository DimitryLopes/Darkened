using System.Numerics;
using UnityEngine.Events;

public abstract class EnemyState<U> : IEnemyState where U : BaseEnemyStateData
{
    private bool isActive;
    private UnityAction onDeactivateCallback;
    private UnityAction onActivateCallback;

    protected EnemyBase Enemy => Data.Enemy;
    public bool IsActive => isActive;
    protected U Data { get; private set; }

    public abstract void HandleState();

    public void SetUp<T>(T data) where T : BaseEnemyStateData
    {
        Data = data as U;
    }

    public void Activate()
    {
        if (!isActive)
        {
            isActive = true;
            OnActivate();
        }
    }

    public void Deactivate()
    {
        if (isActive)
        {
            isActive = false;
            OnDeactivate();
        }
    }

    public virtual void OnDeactivate()
    {
        onDeactivateCallback?.Invoke();
    }
    public virtual void OnActivate() 
    {
        onActivateCallback?.Invoke();
    }

    public void SetOnDeactivateCallback(UnityAction callback)
    {
        onDeactivateCallback = callback;
    }

    public void SetOnActivateCallback(UnityAction callback)
    {
        onActivateCallback = callback;
    }
}
