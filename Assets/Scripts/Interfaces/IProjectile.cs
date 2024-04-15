using UnityEngine;

public interface IProjectile
{
    float TravelTime { get; }
    float Speed { get; }
    void OnHit(Collider2D collision);
    void Shoot(Vector2 direction);
}
