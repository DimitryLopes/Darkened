using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableItem : MissionItem
{
    protected override void OnInteract()
    {
        signalBus.Fire(new OnInventoryItemGetSignal(this, 1));
    }
}
