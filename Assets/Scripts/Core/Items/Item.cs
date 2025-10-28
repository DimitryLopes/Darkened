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
    private Collider2D itemCollider;
    [SerializeField]
    private SpriteAnimator spriteAnimator;
    [SerializeField]
    private ItemGenerationData itemGenerationData;

    protected ItemType type;
    protected string defaultAnimationKey = "default";

    public virtual Sprite Icon => itemIcon;
    public virtual ItemType Type => type;
    public Collider2D Collider => itemCollider;
    public SpriteAnimator SpriteAnimator => spriteAnimator;

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
        base.OnActivate();
    }

    public override void OnDeactivate()
    {
        canInteract = false;
    }

    public void Highlight()
    {
        if (canInteract)
        {
            spriteRenderer.material = materialManager.GetMaterial(MaterialType.Outline);
        }
    }

    public void RemoveHighlight()
    {
        spriteRenderer.material = materialManager.GetMaterial(MaterialType.Default);
    }

    protected virtual void PlayDefaultAnimation()
    {
        SpriteAnimator.PlayAnimation(defaultAnimationKey, PlayDefaultAnimation);
    }

    protected virtual void SetDefaultAnimationKey() { }
}

[Serializable]
public struct ItemGenerationData
{
    [SerializeField, Tooltip("In nodes")]
    private int minDistanceFromThings;
    [SerializeField]
    private SpawnType spawnType;

    public SpawnType SpawnType => spawnType;
    public int MinDistanceFromThings => minDistanceFromThings;
}

public enum SpawnType
{
    Node,
    Wall,
    EdgeWalls
}

