using UnityEngine;
using Zenject;

public abstract class Enemy : Activateable, IStateUser
{
    [SerializeField]
    private Collider2D enemyCollider;
    [SerializeField]
    private EnemyData data;
    [SerializeField]
    private Rigidbody2D rb;

    [Inject]
    protected EnemyArrowPointer arrowPointer;

    protected CameraManager cameraManager;
    protected AudioManager audioManager;
    protected IState currentState;
    protected SignalBus signalBus;
    protected Player Player;
    protected Maze Maze;
    protected Distraction distraction;
    protected Rigidbody2D Rb => rb;

    protected bool IsDistracted => distraction != null;
    public float EnhancedDetectionRange => data.EnhancedDetectionRange;
    public float DetectionRange => data.DetectionRange;
    public float MovementSpeed => data.Speed;
    public float SprintingSpeed => data.Speed * data.SprintingSpeedMultiplier;
    public Transform Transform => transform;

    [Inject]
    public virtual void Initialize(EntityManager entityManager, CameraManager cameraManager, AudioManager audioManager,
        SignalBus signalBus)
    {
        this.audioManager = audioManager;
        Player = entityManager.GetPlayer();
        this.cameraManager = cameraManager;
        this.signalBus = signalBus;

        signalBus.Subscribe<OnEnemyHitSignal>(OnHit);
    }

    public void DeactivateCollider()
    {
        enemyCollider.enabled = false;
    }

    public void ActivateCollider()
    {
        enemyCollider.enabled = true;
    }

    public virtual void SetMaze(Maze maze)
    {
        Maze = maze;
    }

    protected virtual void ChangeState(IState state)
    {
        if (state == currentState) return;

        currentState?.Deactivate();
        currentState = state;
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

    protected virtual void OnHit(OnEnemyHitSignal signal)
    {
        switch (signal.Item.Type)
        {
            case ItemType.Arrow:
                Deactivate();
                return;
            case ItemType.Distraction:
                OnDistracted(signal.Item as Distraction);
                return;
        }
    }

    private void Update()
    {
        currentState.HandleState();
    }

    protected virtual void OnDistracted(Distraction distraction) { }
    protected virtual void OnDistractionEnded() { }
}
