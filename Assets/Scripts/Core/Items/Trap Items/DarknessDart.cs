using UnityEngine;
using Zenject;

public class DarknessDart : Item, IProjectile
{
    [Inject]
    private EntityManager entityManager;
    [Inject]
    private MazeManager mazeManager;

    [SerializeField]
    private Rigidbody2D rb;
    [SerializeField]
    private float travelTime;
    [SerializeField]
    private float speed;

    public float TravelTime => travelTime;
    public float Speed => speed;

    public override ItemType Type => ItemType.PoisonDart;

    public void OnHit(Collider2D collision)
    {
        if (collision.CompareTag(Constants.LayersAndTags.PLAYER_TAG))
        {
            float torchReduction = mazeManager.CurrentMaze.Data.DifficultyData.DarknessDartTorchReductionOnHit;
            entityManager.GetPlayer().Torch.ChangeCurrentLifeTime(-torchReduction);
        }
        Deactivate();
    }

    public void Shoot(Vector2 direction)
    {
        Activate();
        rb.velocity = direction * speed;

        float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnHit(collision);
    }
}
