public class OnGameCompletedSignal
{
    public bool Won { get; private set; }

    public OnGameCompletedSignal(bool won)
    {
        Won = won;
    }
}
