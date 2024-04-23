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
    private MaterialType standardtMaterial;
    [SerializeField]
    private MaterialType highlightedtMaterial;
    [SerializeField]
    private ItemGenerationData itemGenerationData;

    protected ItemType type;

    public Sprite Icon => itemIcon;
    public ItemType Type => type;

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
            spriteRenderer.material = materialManager.GetMaterial(highlightedtMaterial);
        }
    }

    public void RemoveHighlight()
    {
        if (canInteract)
        {
            spriteRenderer.material = materialManager.GetMaterial(standardtMaterial);
        }
    }

}

[Serializable]
public struct ItemGenerationData
{
    [SerializeField, Tooltip("In nodes")]
    private int minDistanceFromStart;
    [SerializeField, Tooltip("In nodes")]
    private int minDistanceFromOtherItems;
    [SerializeField]
    private SpawnType spawnType;
    [SerializeField, Tooltip("wether the item will replace or not the wall/node in it's position")]
    private bool replace;

    public SpawnType SpawnType => spawnType;
    public bool Replace => replace;
    public int MinDistanceFromOtherItems => minDistanceFromOtherItems;
    public int MinDistanceFromStart => minDistanceFromStart;
}

public enum SpawnType
{
    Node,
    Wall,
    EdgeWalls
}

