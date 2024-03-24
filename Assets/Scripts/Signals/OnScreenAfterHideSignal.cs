public class OnScreenAfterHideSignal
{
    public IScreen Screen { get; private set; }

    public OnScreenAfterHideSignal(IScreen screen)
    {
        Screen = screen;
    }

}
