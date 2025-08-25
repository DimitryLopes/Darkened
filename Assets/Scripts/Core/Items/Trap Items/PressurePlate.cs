using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : TrapItem, IToggable
{
    private List<TrapItem> traps = new();

    [SerializeField]
    private Sprite activatedSprite;
    [SerializeField]
    private Sprite deactivatedSprite;

    public override ItemType Type => ItemType.PressurePlate;

    public bool Toggled { get; private set; }

    public void Toggle(MazeManager mazeManager)
    {
        Toggled = !Toggled;
        
        if (Toggled)
        {
            Trigger();
        }

        spriteRenderer.sprite = Toggled ? activatedSprite : deactivatedSprite;
    }

    public void Associate(TrapItem trap)
    {
        traps.Add(trap);
    }

    public override void Trigger()
    {
        foreach (var trap in traps)
        {
            trap.Trigger();
        }
    }

    public override void OnActivate()
    {
        base.OnActivate();
        Toggled = false;
        spriteRenderer.sprite = deactivatedSprite;
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();
        traps.Clear();
    }
}
