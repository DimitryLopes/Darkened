using UnityEngine;
using Zenject;

public class Distraction : UsableItem
{
    [Inject]
    private EntityManager entityManager;
    [Inject]
    private InventoryManager inventoryManager;

    public override ItemType Type => ItemType.Distraction;

    public override bool CanUse => inventoryManager.HasEnoughItem(Type);

    public override void OnItemUsed()
    {
        inventoryManager.DecreaseItemAmount(Type);

        Player player = entityManager.GetPlayer();
        transform.position = player.transform.position;
        Activate();
        DisableInteraction();

        signalBus.Fire(new OnEnemyHitSignal(this));
    }
}
