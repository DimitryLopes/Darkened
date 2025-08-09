using UnityEngine;
using Zenject;

public class Energetic : UsableItem
{
    [Inject]
    private EntityManager entityManager;
    [Inject]
    private InventoryManager inventoryManager;
    [SerializeField]
    public StatusEffectData StatusEffectData;

    public override ItemType Type => ItemType.Wisp;

    public override bool CanUse => inventoryManager.HasEnoughItem(Type);

    public override void OnItemUsed()
    {
        inventoryManager.DecreaseItemAmount(Type);

        Player player = entityManager.GetPlayer();
        player.ApplyStatusEffect(StatusEffectData);
    }
}
