public class OnTutorialStartedSignal
{
    public Tutorial Tutorial { get; private set; }

    public OnTutorialStartedSignal(Tutorial tutorial) 
    {
        Tutorial = tutorial;
    }
}
