using System.Collections.Generic;

public class OnMazeChangedDinamicallySignal 
{
    public List<IToggable> ToggledItems { get; private set; }

    public OnMazeChangedDinamicallySignal(List<IToggable> toggledItems)
    {
        ToggledItems = toggledItems;
    }
}
