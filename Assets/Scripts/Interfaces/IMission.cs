public interface IMission: IActivateable
{
    string Description { get; }
    int RawProgress { get; }
    bool IsCompleted { get; }
    void ForceComplete();
    public float GetCurrentProgress();
    public int GetTargetProgress(); 
    void SetUp<T>(T data, DifficultyType difficulty) where T : MissionData;
}
