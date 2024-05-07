using Zenject;

public class Wisp : UsableItem
{
    [Inject]
    private EntityManager entityManager;
    [Inject]
    private InventoryManager inventoryManager;

    public override bool CanUse => inventoryManager.HasEnoughItem(Type);

    public override void OnItemUsed()
    {
        inventoryManager.DecreaseItemAmount(Type);

        Player player = entityManager.GetPlayer();
        player.SetSpectralTorch();
    }
}
