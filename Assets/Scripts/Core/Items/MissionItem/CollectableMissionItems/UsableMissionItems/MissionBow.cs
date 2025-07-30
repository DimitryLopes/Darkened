using Zenject;

public class MissionBow : UsableMissionItem
{
    [Inject]
    private EntityManager entityManager;
    [Inject]
    private InventoryManager inventoryManager;

    public override bool CanUse => inventoryManager.HasEnoughItem(ItemType.Arrow);

    public override void OnItemUsed()
    {
        MissionArrow arrow = inventoryManager.GetItem<MissionArrow>(ItemType.Arrow);
        if (arrow == null) return;

        inventoryManager.DecreaseItemAmount(ItemType.Arrow);

        Player player = entityManager.GetPlayer();
        arrow.transform.position = player.transform.position;
        arrow.Shoot(player.transform.up);
    }
}
