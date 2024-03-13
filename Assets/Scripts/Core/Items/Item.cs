using System;
using UnityEngine;
public class Item : Activateable, IItem, IInteractable
{
    [SerializeField]
    private ItemGenerationData itemGenerationData;
    protected ItemType type;

    public ItemGenerationData GenerationData => itemGenerationData;

    protected bool canInteract;
    public bool CanInteract => canInteract;

    public virtual void Interact()
    {
    }

    public void SetType(ItemType type)
    {
        this.type = type;
    }

    protected override void OnActivate()
    {
        canInteract = true;
    }

    protected override void OnDeactivate()
    {
        canInteract = false;
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

