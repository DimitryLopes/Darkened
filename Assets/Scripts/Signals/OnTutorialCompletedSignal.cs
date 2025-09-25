public class OnTutorialCompletedSignal
{
    public Tutorial Tutorial { get; private set; }

    public OnTutorialCompletedSignal(Tutorial tutorial) 
    {
        Tutorial = tutorial;
    }
}
