using Zenject;
using UnityEngine;

public class EnemyArrowPointer : Activateable
{
    [Inject]
    private SignalBus signalBus;
    [Inject]
    private EntityManager entityManager;

    [SerializeField]
    private float minDistanceToBeActive;
    [SerializeField]
    private float maxDistanceToBeActive;
    [SerializeField]
    private float offset;
    [SerializeField]
    public RectTransform arrow;

    private Transform player;
    private Transform enemy;
    private Camera mainCamera;
    private float currentDistance;

    private void Start()
    {
        mainCamera = Camera.main;
        signalBus.Subscribe<OnMazeLoadFinishSignal>(OnMazeLoadFinish);
        RawDeactivate();
    }


    void LateUpdate()
    {
        if (player != null && enemy != null && IsActive)
        {
            Vector3 playerScreenPos = mainCamera.WorldToScreenPoint(player.position);
            Vector3 enemyScreenPos = mainCamera.WorldToScreenPoint(enemy.position);

            Vector3 direction = enemyScreenPos - playerScreenPos;  // Direction from player to enemy
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;  // Calculate the angle

            // Set the position of the arrow around the player
            arrow.position = playerScreenPos + new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0) * offset;
            arrow.rotation = Quaternion.Euler(0, 0, angle);  // Rotate the arrow

            float size = Mathf.Clamp01((maxDistanceToBeActive - currentDistance) / (maxDistanceToBeActive - minDistanceToBeActive));
            arrow.localScale = new Vector3(size, size, 1);
        }
    }


    private void OnMazeLoadFinish()
    {
        player = entityManager.GetPlayer().transform;
        enemy = entityManager.GetCurrentEnemy().transform;
        ToggleArrow();//enemy does that, but it might be good to leave this here anyway
    }

    public void ToggleArrow(float distance = 0)
    {
        currentDistance = distance;
        if (distance >= minDistanceToBeActive && distance < maxDistanceToBeActive)
            Activate();
        else
            Deactivate();
    }
}
