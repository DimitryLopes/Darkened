public class OnScreenBeforeShowSignal
{
    public IScreen Screen { get; private set; }

    public OnScreenBeforeShowSignal(IScreen screen)
    {
        Screen = screen;
    }

}
