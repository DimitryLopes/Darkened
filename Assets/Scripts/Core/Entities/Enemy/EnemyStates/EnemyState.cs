public abstract class EnemyState<T> : IActivateable where T : EnemyStateData
{
    private bool isActive;
    protected EnemyBase enemy;
    protected T Data { get; private set; }

    public bool IsActive => isActive;

    public void SetUp(T data)
    {
        Data = data;
    }

    public abstract void HandleState();

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

    public virtual void OnDeactivate() { }
    public virtual void OnActivate() { }
}
