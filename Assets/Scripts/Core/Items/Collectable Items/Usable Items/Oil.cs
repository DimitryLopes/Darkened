using Zenject;

public class Oil : UsableItem
{
    [Inject]
    private EntityManager entityManager;
    [Inject]
    private InventoryManager inventoryManager;

    public override ItemType Type => base.Type;
    public override bool CanUse => inventoryManager.HasEnoughItem(Type);

    public override void OnItemUsed()
    {
        inventoryManager.DecreaseItemAmount(Type);

        Player player = entityManager.GetPlayer();
        player.SetBigTorch();
    }
}
