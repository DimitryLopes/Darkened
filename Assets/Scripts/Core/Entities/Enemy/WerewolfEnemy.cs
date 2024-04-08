using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class WerewolfEnemy : Enemy
{
    [SerializeField]
    private LayerMask sightLayer;
    [SerializeField]
    private float waitingTime;
    [SerializeField]
    private float raycastOffset;
    [SerializeField]
    private VignetteAnimationData chasePostProcessingEffect;
    [SerializeField]
    private VignetteAnimationData trackingPostProcessingEffect;

    private MovingTowardsTargetEnemyState wanderingState;
    private MovingTowardsTargetEnemyState investigatingState;
    private MovingTowardsTargetEnemyState trackingState;
    private ChasingEnemyState chasingState;
    private WaitingEnemyState waitingState;

    //Updated in real time
    private RaycastHit2D leftHit;
    private RaycastHit2D rightHit;
    private float distanceBetweenPlayer;
    private bool isInLineOfSight;

    private Vector3 LeftRaycastOrigin => transform.position - (Vector3.right * transform.localScale.x) / raycastOffset;
    private Vector3 RightRaycastOrigin => transform.position + (Vector3.right * transform.localScale.x) / raycastOffset;
    private Vector3 LeftRayCastDirection => (Player.transform.position - LeftRaycastOrigin).normalized;
    private Vector3 RightRayCastDirection => (Player.transform.position - RightRaycastOrigin).normalized;

    public override void OnActivate()
    {
        ChangeState(waitingState);
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();
        currentState.RawDeactivate();
    }

    public override void Initialize(MazeManager mazeManager, EntityManager entityManager, CameraManager cameraManager, SignalBus signalBus)
    {
        base.Initialize(mazeManager, entityManager, cameraManager, signalBus);

        BaseEnemyStateData chasingData = new BaseEnemyStateData(Maze, this, true, OnChasingEnded, OnChasingStarted, null);

        chasingState = new ChasingEnemyState(chasingData);
        waitingState = CreateWaitingStateData(OnWaitingCompleted);
        trackingState = CreateMovingStateData(true, null, OnTrackingStarted, OnTrackingComplete);
        wanderingState = CreateMovingStateData(false, null, OnWanderingStarted, OnWanderingCompleted);
        investigatingState = CreateMovingStateData(false, null, OnInvestigationStarted, OnInvestigationCompleted);
    }

    private WaitingEnemyState CreateWaitingStateData(UnityAction onWaitingEnded)
    {
        var data = new WaitingEnemyStateData(Maze, this, false, waitingTime, null, null, onWaitingEnded);
        return new WaitingEnemyState(data);
    }

    private MovingTowardsTargetEnemyState CreateMovingStateData(bool isSprinting, UnityAction onStateDeactivated, UnityAction onStateActivated, UnityAction onStateCompleted)
    {
        var data = new BaseEnemyStateData(Maze, this, isSprinting, onStateDeactivated, onStateActivated, onStateCompleted);
        return new MovingTowardsTargetEnemyState(data);
    }


    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag(Constants.LayersAndTags.PLAYER_TAG)) return;

        signalBus.Fire(new OnGameCompletedSignal(false));
    }

    private void Update()
    {
        currentState.HandleState();

        distanceBetweenPlayer = Vector2.Distance(transform.position, Player.transform.position);

        // Cast raycasts
        leftHit = Physics2D.Raycast(LeftRaycastOrigin, LeftRayCastDirection, DetectionRange, sightLayer);
        rightHit = Physics2D.Raycast(RightRaycastOrigin, RightRayCastDirection, DetectionRange, sightLayer);

        // Debug draw the raycasts
        Debug.DrawRay(LeftRaycastOrigin, LeftRayCastDirection * DetectionRange, Color.blue);
        Debug.DrawRay(RightRaycastOrigin, RightRayCastDirection * DetectionRange, Color.blue);

        //Is not in range
        if (DetectionRange <= distanceBetweenPlayer)
        {
            if (chasingState.IsActive)
            {
                //this also enables Tracking state
                chasingState.Deactivate();
            }
            return;
        }

        isInLineOfSight = leftHit.collider.CompareTag(Constants.LayersAndTags.PLAYER_TAG) && rightHit.collider.CompareTag(Constants.LayersAndTags.PLAYER_TAG);
        
        if (isInLineOfSight)
        {
            ChangeState(chasingState);
            return;
        }
        //No longer in LOS

        if (chasingState.IsActive)
        {
            //this also enables Tracking state
            chasingState.Deactivate();
            return;
        }

        if (trackingState.IsActive) return;

        //Lost track of player
        ChangeState(investigatingState);
        return;
    }

    private void SetTrackingPath()
    {
        trackingState.SetPath(Color.red, GetPlayerNode());
    }

    private MazeNode GetPlayerNode()
    {
        MazeNode playerNode = MazeUtils.GetClosestNodeToVector(Player.transform.position, Maze.Nodes);
        return playerNode;
    }

    #region Callbacks
    private void OnChasingStarted()
    {
        cameraManager.AnimateVignette(chasePostProcessingEffect);
        chasingState.SetPath(Player.transform);
    }

    private void OnChasingEnded()
    {
        cameraManager.FinishVignetteAnimation(chasePostProcessingEffect);
        ChangeState(trackingState);
    }

    private void OnTrackingStarted()
    {
        cameraManager.AnimateVignette(trackingPostProcessingEffect);
        SetTrackingPath();
    }

    private void OnTrackingComplete()
    {
        cameraManager.FinishVignetteAnimation(trackingPostProcessingEffect);
        ChangeState(waitingState);
    }

    private void OnInvestigationStarted()
    {
        investigatingState.SetPath(Color.yellow, GetPlayerNode());
    }

    private void OnInvestigationCompleted()
    {
        ChangeState(waitingState);
    }

    private void OnWanderingStarted()
    {
        wanderingState.SetPath(Color.grey);
    }

    private void OnWanderingCompleted()
    {
        ChangeState(waitingState);
    }


    private void OnWaitingCompleted()
    {
        ChangeState(wanderingState);
    }
    #endregion
}
