using UnityEngine;
using Zenject;

public class PoisonDart : TrapItem, IProjectile
{
    [Inject]
    private EntityManager entityManager;

    [SerializeField]
    private Rigidbody2D rb;
    [SerializeField]
    private float travelTime;
    [SerializeField]
    private float speed;
    [SerializeField]
    private StatusEffectData effectData;

    public float TravelTime => travelTime;
    public float Speed => speed;

    public void OnHit(Collider2D collision)
    {
        if (collision.CompareTag(Constants.LayersAndTags.PLAYER_TAG))
        {
            entityManager.GetPlayer().ApplyStatusEffect(effectData);
        }
        Deactivate();
    }

    public override void Trigger()
    {
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
