using UnityEngine;
using Zenject;

public class Distraction : UsableItem
{
    [Inject]
    private EntityManager entityManager;
    [Inject]
    private InventoryManager inventoryManager;
    [SerializeField]
    private float travelTime;
    [SerializeField]
    private float speed;

    [SerializeField]
    private Rigidbody2D rb;

    public override bool CanUse => inventoryManager.HasEnoughItem(ItemType.Distraction);

    public float TravelTime => travelTime;

    public float Speed => speed;

    public override void OnItemUsed()
    {
        inventoryManager.DecreaseItemAmount(ItemType.Distraction);

        Player player = entityManager.GetPlayer();
        transform.position = player.transform.position;

        signalBus.Fire(new OnEnemyHitSignal(this));
    }
}
