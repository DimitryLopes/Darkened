using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    [SerializeField]
    private EnemyData data;
    [SerializeField]
    private new Rigidbody2D rb;

    protected IEnemyState currentState;

    public float MovementSpeed => data.Speed;
    public float SprintingSpeed => data.Speed * data.SprintingSpeedMultiplier;

    protected virtual void ChangeState(IEnemyState state)
    {
        currentState.Deactivate();
        currentState = state;
        currentState.Activate();
    }

    public void Move(Vector3 target, bool isSprinting = false)
    {
        Vector3 direction = target - transform.position;
        direction *= isSprinting ? SprintingSpeed : MovementSpeed;
        rb.velocity = direction;
    }

    private void Update()
    {
        currentState.HandleState();
    }
}
