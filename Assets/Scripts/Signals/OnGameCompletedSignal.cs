public class OnGameCompletedSignal
{
    public Objective Objective { get; private set; }
    public bool Won { get; private set; }

    public OnGameCompletedSignal(Objective objective, bool won)
    {
        Objective = objective;
        Won = won;
    }
}
