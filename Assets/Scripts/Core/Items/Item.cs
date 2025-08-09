using System;
using UnityEngine;
using Zenject;

public class Item : Activateable, IItem, IInteractable
{
    [Inject]
    protected SignalBus signalBus;
    [Inject]
    protected MaterialManager materialManager;

    [SerializeField]
    private Sprite itemIcon;
    [SerializeField]
    protected SpriteRenderer spriteRenderer;
    [SerializeField]
    private ItemGenerationData itemGenerationData;

    protected ItemType type;

    public virtual Sprite Icon => itemIcon;
    public virtual ItemType Type => type;

    public ItemGenerationData GenerationData => itemGenerationData;

    protected bool canInteract;
    public bool CanInteract => canInteract;

    public virtual void Interact()
    {
        if (CanInteract)
        {
            OnInteract();
        }
    }

    protected virtual void OnInteract() { }

    public void SetType(ItemType type)
    {
        this.type = type;
    }

    public override void OnActivate()
    {
        canInteract = true;
        RemoveHighlight();
    }

    public override void OnDeactivate()
    {
        canInteract = false;
    }

    public void Highlight()
    {
        if (canInteract)
        {
            spriteRenderer.material = materialManager.GetMaterial(MaterialType.Default);
        }
    }

    public void RemoveHighlight()
    {
        if (canInteract)
        {
            spriteRenderer.material = materialManager.GetMaterial(MaterialType.Outline);
        }
    }

}

[Serializable]
public struct ItemGenerationData
{
    [SerializeField, Tooltip("In nodes")]
    private int minDistanceFromThings;
    [SerializeField]
    private SpawnType spawnType;
    [SerializeField, Tooltip("wether the item will replace or not the wall/node in it's position")]
    private bool replace;

    public SpawnType SpawnType => spawnType;
    public bool Replace => replace;
    public int MinDistanceFromThings => minDistanceFromThings;
}

public enum SpawnType
{
    Node,
    Wall,
    EdgeWalls
}

