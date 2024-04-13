using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class MazeBow : UsableItem
{
    [Inject]
    private InventoryManager inventoryManager;

    public override bool CanUse => inventoryManager.HasEnoughItem(ItemType.Arrow);

    protected override void OnItemUsed()
    {
        inventoryManager.ReduceItemAmount(ItemType.Arrow);
        //shoot the arrow
    }

    protected override void OnItemUseConditionChanged()
    {
        throw new System.NotImplementedException();
    }
}
