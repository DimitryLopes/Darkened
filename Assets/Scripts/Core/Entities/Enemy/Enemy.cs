using UnityEngine;
using UnityEngine.Events;
using Zenject;

public abstract class Enemy : Activateable
{
    [SerializeField]
    private EnemyData data;
    [SerializeField]
    private new Rigidbody2D rb;

    protected IEnemyState currentState;
    protected SignalBus signalBus;
    protected Player Player;
    protected Maze Maze;

    public float EnhancedDetectionRange => data.EnhancedDetectionRange;
    public float DetectionRange => data.DetectionRange;
    public float MovementSpeed => data.Speed;
    public float SprintingSpeed => data.Speed * data.SprintingSpeedMultiplier;

    [Inject]
    public virtual void Initialize(MazeManager mazeManager, EntityManager entityManager, SignalBus signalBus)
    {
        Maze = mazeManager.CurrentMaze;
        Player = entityManager.GetPlayer();
        this.signalBus = signalBus;
    }

    protected virtual void ChangeState(IEnemyState state)
    {
        if (state == currentState) return;

        currentState?.Deactivate();
        currentState = state;
        Debug.Log($"Changed state to {state}");
        currentState.Activate();
    }

    public void Move(Vector3 target, bool isSprinting = false)
    {
        Vector3 direction = target.normalized;
        direction *= isSprinting ? SprintingSpeed : MovementSpeed;
        rb.velocity = direction;

        if (target != Vector3.zero)
        {
            float angle = Mathf.Atan2(-direction.x, direction.y) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
        }
    }

    private void Update()
    {
        currentState.HandleState();
    }
}
