using UnityEngine;
using Zenject;

public class MazeArrow : CollectableMissionItem, IProjectile
{

    [SerializeField]
    private Rigidbody2D rb;
    [SerializeField]
    private float travelTime;
    [SerializeField]
    private float speed;

    public float TravelTime => travelTime;
    public float Speed => speed;

    public void OnHit(Collider2D collision)
    {
        if (
            collision.CompareTag(Constants.LayersAndTags.WALL_TAG) 
            || collision.CompareTag(Constants.LayersAndTags.WALL_ITEM_REPLACE_TAG)
        )
        {
            rb.velocity = Vector2.zero;
            EnableInteraction();
        }
        else if (collision.CompareTag(Constants.LayersAndTags.ENEMY_TAG))
        {
            signalBus.Fire(new OnEnemyHitSignal());
        }
    }

    public void Shoot(Vector2 direction)
    {
        Activate();
        DisableInteraction();
        rb.velocity = direction * speed;

        float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    protected override void OnInteract()
    {
        base.OnInteract();
        Deactivate();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (CanInteract) return;
        OnHit(collision);
    }
}
