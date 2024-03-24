public class OnScreenBeforeHideSignal
{
    public IScreen Screen { get; private set; }

    public OnScreenBeforeHideSignal(IScreen screen)
    {
        Screen = screen;
    }

}
