public interface IMission: IActivateable
{
    int ProgressTarget { get; }
    string Description { get; }
    float Progress { get; }
    int RawProgress { get; }
    bool IsCompleted { get; }
    void ForceComplete();

    void SetUp<T>(T data) where T : MissionData;
}
