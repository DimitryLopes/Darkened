using UnityEngine;
using Zenject;

public class WerewolfEnemy : Enemy
{
    [SerializeField]
    private LayerMask wallLayer;
    [SerializeField]
    private float waitingTime;

    private WanderingEnemyState wanderingState;
    private InvestigantingEnemyState investigatingState;
    private ChasingEnemyState chasingState;
    private WaitingEnemyState waitingState;

    private RaycastHit2D raycastHit;
    private float distanceBetweenPlayer;
    private bool isInLineOfSight;

    public override void OnActivate()
    {
        ChangeState(wanderingState);
    }

    public override void Initialize(MazeManager mazeManager, EntityManager entityManager, SignalBus signalBus)
    {
        base.Initialize(mazeManager, entityManager, signalBus);

        BaseEnemyStateData stateData = new BaseEnemyStateData(Maze, this);
        WaitingEnemyStateData waitingStateData = new WaitingEnemyStateData(Maze, this, waitingTime);

        waitingState = new WaitingEnemyState();
        chasingState = new ChasingEnemyState();
        wanderingState = new WanderingEnemyState();
        investigatingState = new InvestigantingEnemyState();

        chasingState.SetUp(stateData);
        wanderingState.SetUp(stateData);
        investigatingState.SetUp(stateData);
        waitingState.SetUp(waitingStateData);

        chasingState.SetOnActivateCallback(OnChasingStarted);
        chasingState.SetOnDeactivateCallback(OnChasingEnded);

        investigatingState.SetOnActivateCallback(OnInvestigationStarted);
        investigatingState.SetOnDeactivateCallback(OnInvestigationEnded);

        waitingState.SetOnDeactivateCallback(OnWaitingEnded);
        wanderingState.SetOnDeactivateCallback(OnWanderingEnded);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(!collision.CompareTag(Constants.LayersAndTags.PLAYER_TAG))
        {
            return;
        }
        signalBus.Fire(new OnGameCompletedSignal(false));
    }

    private void Update()
    {
        currentState.HandleState();

        distanceBetweenPlayer = Vector2.Distance(transform.position, player.transform.position);

        raycastHit = Physics2D.Linecast(transform.position, player.transform.position, wallLayer);
        Debug.DrawLine(transform.position, raycastHit.point, Color.red, 0.1f);

        if (DetectionRange <= distanceBetweenPlayer) return;

        isInLineOfSight = raycastHit.collider == null;
        if (!isInLineOfSight)
        {
            ChangeState(investigatingState);
            return;
        }

        ChangeState(chasingState);
    }

    #region Callbacks
    private void OnInvestigationEnded()
    {
        ChangeState(wanderingState);
    }

    private void OnChasingEnded()
    {
        ChangeState(wanderingState);
    }

    private void OnWanderingEnded()
    {
        ChangeState(waitingState);
    }

    private void OnWaitingEnded()
    {
        ChangeState(wanderingState);
    }

    private void OnInvestigationStarted()
    {
        investigatingState.SetPath(MazeUtils.GetClosestNodeToVector(player.transform.position, Maze.Nodes));
    }

    private void OnChasingStarted()
    {
        chasingState.SetPath(player.transform);
    }
    #endregion
}
