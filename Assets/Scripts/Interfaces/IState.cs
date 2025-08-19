public interface IState : IActivateable
{
    void RawDeactivate();
    void HandleState();
}
