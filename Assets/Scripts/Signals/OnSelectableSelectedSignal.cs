public class OnSelectableSelectedSignal
{
    public IUISelectable Selectable { get; private set; }

    public OnSelectableSelectedSignal(IUISelectable selectable)
    {
        Selectable = selectable;
    }
}