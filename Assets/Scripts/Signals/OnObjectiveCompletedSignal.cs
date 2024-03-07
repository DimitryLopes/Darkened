public class OnObjectiveCompletedSignal
{
    public Objective Objective { get; private set; }

    public OnObjectiveCompletedSignal(Objective objective)
    {
        Objective = objective;
    }
}
