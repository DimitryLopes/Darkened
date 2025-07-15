using Zenject;

public abstract class Mission<T> : IMission where T : MissionData
{
    protected SignalBus signalBus;
    protected int progress;
    protected DifficultyType Difficulty;

    public string Description => Data.Description;
    public T Data { get; protected set; }
    public int RawProgress => progress;
    public bool IsCompleted { get; private set; }
    public bool IsActive { get; set; }

    protected virtual void UpdateProgress()
    {
        signalBus.Fire(new OnMissionProgressSignal(this));
        if (progress == GetTargetProgress())
        {
            CompleteMission();
        }
    }

    public int GetTargetProgress()
    {
        return Data.GetTargetProgress(Difficulty);
    }

    public float GetCurrentProgress()
    {
        return (float)progress / (float)GetTargetProgress();
    }

    public void ForceComplete()
    {
        IsCompleted = true;
        OnMissionCompleted();
    }

    protected void CompleteMission()
    {
        OnMissionCompleted();
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

    protected virtual void OnActivate() 
    {
        OnMissionStarted();
    }

    protected virtual void OnDeactivate() { }

    protected virtual void OnMissionStarted() 
    {
        progress = 0;
        IsCompleted = false;
    }

    protected virtual void OnMissionCompleted()
    {
        progress = Data.GetTargetProgress(Difficulty);
        IsCompleted = true;
    }

    public void SetUp<U>(U data, DifficultyType difficulty) where U : MissionData
    {
        Data = data as T;
        Difficulty = difficulty;
    }

    public Mission(SignalBus signalBus)
    {
        this.signalBus = signalBus;
        progress = 0;
        IsActive = false;
    }
}
