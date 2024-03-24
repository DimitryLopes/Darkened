public interface ILoadable
{
    float Progress { get; }
    bool IsComplete { get; }

    void Load();
}
