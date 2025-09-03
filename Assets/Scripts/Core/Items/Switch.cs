using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Switch : Item, IToggable
{
    [Inject]
    private MazeManager mazeManager;

    [SerializeField]
    private Sprite OnSpire;
    [SerializeField]
    private Sprite OffSpire;

    public override ItemType Type => ItemType.Switch;

    public bool Toggled { get; private set; }

    private List<IToggable> toggles = new();

    public override void OnActivate()
    {
        base.OnActivate();
        Toggled = false;
        spriteRenderer.sprite = OffSpire;
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();
        toggles.Clear();
    }

    protected override void OnInteract()
    {
        Toggle(mazeManager);
    }

    public void Associate(IToggable toggable)
    {
        toggles.Add(toggable);
    }

    public void Toggle(MazeManager mazeManager)
    {
        foreach (IToggable toggble in toggles)
        {
            toggble.Toggle(mazeManager);
        }


        signalBus.Fire(new OnMazeChangedDinamicallySignal(toggles));

        Toggled = !Toggled;
        spriteRenderer.sprite = Toggled ? OnSpire : OffSpire;
    }
}
