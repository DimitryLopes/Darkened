public interface IEnemyState : IActivateable
{
    void SetUp<T>(T data) where T : BaseEnemyStateData;
    void HandleState();
}
