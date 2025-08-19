using UnityEngine;
using Zenject;

public class Spirit : MissionItem, IStateUser
{
    [Inject]
    private MazeManager mazeManager;

    [SerializeField]
    private Rigidbody2D rb;
    [SerializeField]
    private float movementSpeed;

    public override ItemType Type => ItemType.Spirit;

    private MovingTowardsTargetState movingTowardsTargetState;
    public Transform Transform => transform;

    public override void OnActivate()
    {
        base.OnActivate();
        BaseStateData data = new BaseStateData(this, false, MoveToAnotherNode, null, MoveToAnotherNode);
        movingTowardsTargetState = new MovingTowardsTargetState(data);
        movingTowardsTargetState.Activate();
    }

    private void MoveToAnotherNode()
    {
        movingTowardsTargetState.SetPath(mazeManager.CurrentMaze);
    }

    public void Move(Vector3 target, bool isSprinting = false)
    {
        Vector3 direction = target.normalized;
        rb.velocity = direction * movementSpeed * mazeManager.CurrentMaze.Data.DifficultyData.SpiritMovementSpeedModifier;

        if (target != Vector3.zero)
        {
            float angle = Mathf.Atan2(-direction.x, direction.y) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
        }
    }

    public override void Interact()
    {
        base.Interact();
        Deactivate();
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();
        movingTowardsTargetState.Deactivate();
    }
}
