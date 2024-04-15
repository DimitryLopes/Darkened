using Zenject;

public class MazeBow : UsableMissionItem
{
    [Inject]
    private EntityManager entityManager;
    [Inject]
    private InventoryManager inventoryManager;

    public override bool CanUse => inventoryManager.HasEnoughItem(ItemType.Arrow);

    public override void OnItemUsed()
    {
        MazeArrow arrow = inventoryManager.GetItem<MazeArrow>(ItemType.Arrow);
        if (arrow == null) return;

        inventoryManager.DecreaseItemAmount(ItemType.Arrow);

        Player player = entityManager.GetPlayer();
        arrow.transform.position = player.transform.position;
        arrow.Shoot(player.transform.up);
    }
}
