public interface IEnemyState : IActivateable
{
    void RawDeactivate();
    void HandleState();
}
