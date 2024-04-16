using Zenject;

public abstract class Mission<T> : IMission where T : MissionData
{
    protected SignalBus signalBus;
    protected int progress;

    public string Description => Data.Description;
    public int ProgressTarget => Data.ProgressTarget;
    protected T Data { get; set; }
    public virtual float Progress { get; protected set; }
    public int RawProgress => progress;
    public bool IsCompleted { get; private set; }
    public bool IsActive { get; set; }

    protected void CompleteMission()
    {
        IsCompleted = true;
        signalBus.Fire(new OnMissionCompletedSignal(this));
    }

    public void Activate()
    {
        if (IsActive) return;
        IsActive = true;
        OnActivate();
    }


    public void Deactivate()
    {
        if (!IsActive) return;
        IsActive = false;
        OnDeactivate();
    }

    protected virtual void OnActivate() { }

    protected virtual void OnDeactivate() { }
    protected virtual void OnMissionStarted() 
    {
        progress = 0;
        IsCompleted = false;
    }

    public void SetUp<U>(U data) where U : MissionData
    {
        Data = data as T;
    }

    public void StartMission()
    {
        Activate();
        OnMissionStarted();
    }

    public Mission(SignalBus signalBus)
    {
        this.signalBus = signalBus;
        progress = 0;
        IsActive = false;
    }
}
