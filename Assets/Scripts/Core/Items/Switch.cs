using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Switch : Item
{
    [Inject]
    private MazeManager mazeManager;

    [SerializeField]
    private Sprite OnSpire;
    [SerializeField]
    private Sprite OffSpire;

    bool isOn = false;

    public override ItemType Type => ItemType.Switch;

    private List<IToggable> toggles = new();
    
    protected override void OnInteract()
    {
        foreach (IToggable gate in toggles)
        {
            gate.Toggle(mazeManager);
        }
        isOn = !isOn;
        spriteRenderer.sprite = isOn ? OnSpire : OffSpire;
    }

    public void Associate(IToggable toggable)
    {
        spriteRenderer.sprite = OffSpire;
        isOn = false;
        toggles.Add(toggable);
    }
}
