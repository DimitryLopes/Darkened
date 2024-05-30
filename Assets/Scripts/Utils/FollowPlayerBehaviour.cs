using UnityEngine;
using Zenject;

public class FollowPlayerBehaviour : MonoBehaviour
{
    [Inject]
    private SignalBus signalBus;
    [Inject]
    private EntityManager entityManager;

    [SerializeField]
    private Vector3 offset;

    private Transform followTarget;

    private void Start()
    {
        signalBus.Subscribe<OnMazeLoadFinishSignal>(OnMazeLoadFinish);
    }

    private void LateUpdate()
    {
        if (followTarget != null)
        {
            transform.position = followTarget.position + offset;
        }
    }

    private void OnMazeLoadFinish()
    {
        signalBus.Unsubscribe<OnMazeLoadFinishSignal>(OnMazeLoadFinish);
        followTarget = entityManager.GetPlayer().transform;
    }
}
