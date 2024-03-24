public class OnScreenAfterShowSignal
{
    public IScreen Screen { get; private set; }

    public OnScreenAfterShowSignal(IScreen screen)
    {
        Screen = screen;
    }

}
