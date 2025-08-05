using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class MazeWall : Activateable
{
    [SerializeField]
    private BoxCollider2D boxCollider;

    protected SignalBus signalBus;

    public bool IsAtBorder { get; protected set; }

    public (Node, Node) AdjacentNodes;

    public void AddNode(Node node)
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

    public virtual void AlignWith(Cardinal direction, bool isAtBorder)
    {
        IsAtBorder = isAtBorder;
        float rotation = NodeUtils.GetWallRotationByCardinal(direction);
        transform.rotation = Quaternion.Euler(0, 0, rotation);
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
