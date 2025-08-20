using System;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class WerewolfEnemy : Enemy
{
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private SpriteRenderer effectsRenderer;

    [SerializeField, Header("Config")]
    private LayerMask sightLayer;
    [SerializeField]
    private float waitingTime;
    [SerializeField]
    private float distractionWaitingTime;
    [SerializeField]
    private float raycastOffset;

    [SerializeField, Header("Animations")]
    private float animationDuration;

    [SerializeField, Header("Post Processing")]
    private VignetteAnimationData chasePostProcessingEffect;
    [SerializeField]
    private VignetteAnimationData trackingPostProcessingEffect;

    private MovingTowardsTargetState wanderingState;
    private MovingTowardsTargetState investigatingState;
    private MovingTowardsTargetState trackingState;
    private MovingTowardsTargetState distractedState;
    private ChasingEnemyState chasingState;
    private WaitingEnemyState waitingState;
    private WaitingEnemyState distractedWaitingEnemyState;

    //Updated in real time
    private RaycastHit2D leftHit;
    private RaycastHit2D rightHit;
    private float distanceBetweenPlayer;
    private bool isInLineOfSight;
    private Color effectsRendererColor;

    private Vector3 LeftRaycastOrigin => transform.position - (Vector3.right * transform.localScale.x) / raycastOffset;
    private Vector3 RightRaycastOrigin => transform.position + (Vector3.right * transform.localScale.x) / raycastOffset;
    private Vector3 LeftRayCastDirection => (Player.transform.position - LeftRaycastOrigin).normalized;
    private Vector3 RightRayCastDirection => (Player.transform.position - RightRaycastOrigin).normalized;

    public override void OnActivate()
    {
        distraction = null;
        signalBus.Subscribe<OnMazeChangedDinamicallySignal>(OnMazeChangedDinamically);
        ChangeState(waitingState);
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();
        signalBus.Unsubscribe<OnMazeChangedDinamicallySignal>(OnMazeChangedDinamically);
        currentState.RawDeactivate();
    }

    protected override void OnHit(OnEnemyHitSignal signal)
    {
        cameraManager.ForceFinishAllAnimations();
        base.OnHit(signal);
    }

    public override void Initialize(EntityManager entityManager, CameraManager cameraManager, AudioManager audioManager, SignalBus signalBus)
    {
        effectsRendererColor = Color.red;
        base.Initialize(entityManager, cameraManager, audioManager, signalBus);
        CreateDatas();
    }

    private void CreateDatas()
    {
        var chasingData = new BaseStateData(this, true, OnChasingEnded, OnChasingStarted, null);

        chasingState = new ChasingEnemyState(chasingData);
        waitingState = CreateWaitingStateData(OnWaitingCompleted, waitingTime);
        distractedWaitingEnemyState = CreateWaitingStateData(OnDistractionEnded, distractionWaitingTime);
        trackingState = CreateMovingStateData(true, null, OnTrackingStarted, OnTrackingComplete);
        wanderingState = CreateMovingStateData(false, null, OnWanderingStarted, OnWanderingCompleted);
        distractedState = CreateMovingStateData(true, null, OnDistractionStateStarted, OnDistractionStateEnded);
        investigatingState = CreateMovingStateData(false, null, OnInvestigationStarted, OnInvestigationCompleted);
    }

    private WaitingEnemyState CreateWaitingStateData(UnityAction onWaitingEnded, float duration)
    {
        var data = new WaitingStateData(this, false, duration, null, null, onWaitingEnded);
        return new WaitingEnemyState(data);
    }

    private MovingTowardsTargetState CreateMovingStateData(bool isSprinting, UnityAction onStateDeactivated, UnityAction onStateActivated, UnityAction onStateCompleted)
    {
        var data = new BaseStateData(this, isSprinting, onStateDeactivated, onStateActivated, onStateCompleted);
        return new MovingTowardsTargetState(data);
    }


    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag(Constants.LayersAndTags.PLAYER_TAG)) return;

        signalBus.Fire(new OnGameCompletedSignal(false));
    }

    private void Update()
    {
        currentState.HandleState();

        if (IsDistracted) return;

        distanceBetweenPlayer = Vector2.Distance(transform.position, Player.transform.position);

        // Cast raycasts
        leftHit = Physics2D.Raycast(LeftRaycastOrigin, LeftRayCastDirection, DetectionRange, sightLayer);
        rightHit = Physics2D.Raycast(RightRaycastOrigin, RightRayCastDirection, DetectionRange, sightLayer);

        // Debug draw the raycasts
        Debug.DrawRay(LeftRaycastOrigin, LeftRayCastDirection * DetectionRange, Color.blue);
        Debug.DrawRay(RightRaycastOrigin, RightRayCastDirection * DetectionRange, Color.blue);

        arrowPointer.ToggleArrow(distanceBetweenPlayer);
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
        trackingState.SetPath(Maze, GetPlayerNode());
    }

    private Node GetPlayerNode()
    {
        Node playerNode = MazeUtils.GetClosestNodeToVector(Player.transform.position, Maze.Nodes);
        return playerNode;

        
    }

    private void OnMazeChangedDinamically(OnMazeChangedDinamicallySignal signal)
    {
        if (currentState is not MovingTowardsTargetState movingState) return;

        var path = movingState.Path;
        if (path == null || path.Count == 0) return;

        Node changedNode = signal.Node;
        MazeWall changedWall = signal.Wall;

        Node adjacentWallNode1 = changedWall?.AdjacentNodes.Item1;
        Node adjacentWallNode2 = changedWall?.AdjacentNodes.Item2;

        bool isWallInPath = false;
        if (adjacentWallNode1 != null)
        {
            if (path.Contains(adjacentWallNode1))
            {
                if (adjacentWallNode2 != null)
                {
                    if (path.Contains(adjacentWallNode2))
                    {
                        isWallInPath = true;
                    }
                }
            }
        }

        Node targetNode = changedNode;

        if (isWallInPath)
        {
            foreach(Node node in path)
            {
                if (node == adjacentWallNode1 || node == adjacentWallNode2)
                {
                     targetNode = node;
                    break;
                }
            }
        }


        if (!path.Contains(targetNode)) return;

        // Check if enemy is touching the target node
        Collider2D hit = Physics2D.OverlapCircle(transform.position, 0.1f, LayerMask.GetMask("MazeNode"));
        Node touchingNode = hit?.GetComponent<Node>();

        bool isTouchingChangedNode = touchingNode == targetNode;

        // Convert path to array (top of stack is last in array)
        Node[] pathArray = path.ToArray();
        Node previousNode = null;

        for (int i = 0; i < pathArray.Length - 1; i++)
        {
            if (pathArray[i] == targetNode)
            {
                previousNode = pathArray[i + 1]; // previous in logical path
                break;
            }
        }

        // Bounce back if currently on the changed node
        if (isTouchingChangedNode && previousNode != null)
        {
            transform.position = previousNode.transform.position;
            ChangeState(waitingState);
            return;
        }

        // Stop before target
        if (!isTouchingChangedNode && targetNode != null)
        {
            transform.position = previousNode != null ? previousNode.transform.position : targetNode.transform.position;
            ChangeState(waitingState);
        }
    }



    #region Callbacks
    private void OnChasingStarted()
    {
        if(currentState != trackingState)
        {
            PlaySFX(AudioKey.SFX_enemy_howl);
        }

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
        PlaySFX(AudioKey.SFX_enemy_growl);
        cameraManager.FinishVignetteAnimation(trackingPostProcessingEffect);
        ChangeState(waitingState);
    }

    private void OnInvestigationStarted()
    {

        PlaySFX(AudioKey.SFX_enemy_sniff);
        investigatingState.SetPath(Maze, GetPlayerNode());
    }

    private void OnInvestigationCompleted()
    {
        ChangeState(waitingState);
    }

    private void OnWanderingStarted()
    {
        wanderingState.SetPath(Maze);
    }

    private void OnWanderingCompleted()
    {
        ChangeState(waitingState);
    }

    protected override void OnDistracted(Distraction distraction)
    {
        Node targetNode = MazeUtils.GetClosestNodeToVector(distraction.transform.position, Maze.Nodes);
        distractedState.SetPath(Maze, targetNode);
        ChangeState(distractedState);
        this.distraction = distraction;
    }

    private void OnDistractionStateStarted()
    {
        PlaySFX(AudioKey.SFX_enemy_sniff);
    }

    private void OnDistractionStateEnded()
    {
        ChangeState(distractedWaitingEnemyState);
    }

    private void OnWaitingCompleted()
    {
        ChangeState(wanderingState);
    }

    protected override void OnDistractionEnded()
    {
        ChangeState(wanderingState);
        distraction.Deactivate();
        distraction = null;
    }
    #endregion

    #region Feedback
    private void PlaySFX(AudioKey sfx)
    {
        audioManager.PlaySFX(sfx, false, transform.position);
        DoShowOutlineAnimation(DoHideOutlineAnimation);
    }

    private void DoShowOutlineAnimation(Action callback)
    {
        LeanTweenAnimationData data = new LeanTweenAnimationData(effectsRenderer.gameObject, effectsRenderer.color.a, 1, animationDuration, ChangeEffectAlpha, callback);
        TweenUtils.DoTween(data, finishCurrent: false);
    }

    private void DoHideOutlineAnimation()
    {
        LeanTweenAnimationData data = new LeanTweenAnimationData(effectsRenderer.gameObject, effectsRenderer.color.a, 0, animationDuration, ChangeEffectAlpha);
        TweenUtils.DoTween(data, finishCurrent: false);
    }

    private void ChangeEffectAlpha(float value)
    {
        effectsRendererColor.a = value;
        effectsRenderer.color = effectsRendererColor;
    }
    #endregion
}
