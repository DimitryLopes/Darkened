using System.Collections.Generic;
using Zenject;

public class Switch : Item
{
    [Inject]
    private MazeManager mazeManager;

    private List<IToggable> toggles = new();

    protected override void OnInteract()
    {
        foreach (IToggable gate in toggles)
        {
            gate.Toggle(mazeManager);
        }
    }

    public void Associate(IToggable toggable)
    {
        toggles.Add(toggable);
    }
}
