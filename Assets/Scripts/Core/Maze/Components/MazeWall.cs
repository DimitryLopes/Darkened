using UnityEngine;
using Zenject;

public class MazeWall : Activateable
{
    [SerializeField]
    public float WALL_ITEM_OFFSET = 1f;
    [SerializeField]
    private BoxCollider2D boxCollider;

    protected SignalBus signalBus;

    public bool IsAtBorder { get; private set; }
    public Cardinal AlignedWith { get; private set; }

    public (MazeNode, MazeNode) AdjacentNodes;

    public void AddNode(MazeNode node)
    {
        if(AdjacentNodes.Item1 == null || AdjacentNodes.Item1 == node)
        {
            AdjacentNodes.Item1 = node;
        }
        else if (AdjacentNodes.Item2 == null || AdjacentNodes.Item2 == node)
        {
            AdjacentNodes.Item2 = node;
        }
        else
        {
            Debug.LogError("Cannot add more than two adjacent nodes to a wall.");
        }
    }

    public override void OnActivate()
    {
        gameObject.SetActive(true);
    }

    public override void OnDeactivate()
    {
        gameObject.SetActive(false);
    }

    public void AlignWith(Cardinal direction, bool isAtBorder)
    {
        IsAtBorder = isAtBorder;
        AlignedWith = direction;
        float rotation = NodeUtils.GetWallRotationByCardinal(direction);
        transform.rotation = Quaternion.Euler(0, 0, rotation);
    }

    public void PositionObject(Item item)
    {
        item.transform.rotation = transform.rotation;
        item.transform.position = transform.position + transform.right * WALL_ITEM_OFFSET;
    }

    public virtual void OnWallCreated(SignalBus signalBus)
    {
        this.signalBus = signalBus;
        signalBus.Subscribe<OnItemsLoadFinishSignal>(AddToComposite);
    }

    private void AddToComposite()
    {
        boxCollider.usedByComposite = true;
        signalBus.Unsubscribe<OnItemsLoadFinishSignal>(AddToComposite);
    }

}
