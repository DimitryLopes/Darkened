using UnityEngine;
using Zenject;

public class FakeSpirit : Item, IStateUser
{
    [Inject] 
    private MazeManager mazeManager;

    [SerializeField]
    private Rigidbody2D rb;

    public override ItemType Type =>  ItemType.FakeSpirit;

    private MovingTowardsTargetState movingTowardsTargetState;
    public Transform Transform => transform;

    public override void OnActivate()
    {
        base.OnActivate();
        BaseStateData data = new BaseStateData(this, false, MoveToNode, null, null);
        movingTowardsTargetState = new MovingTowardsTargetState(data);
        MoveToNode();
    }

    private void Update()
    {
        movingTowardsTargetState.HandleState();
    }

    private void MoveToNode()
    {
        movingTowardsTargetState.SetPath(mazeManager.CurrentMaze);
        movingTowardsTargetState.Activate();
    }

    public void Move(Vector3 target, bool isSprinting = false)
    {
        Vector3 direction = target.normalized;
        rb.velocity = direction * mazeManager.CurrentMaze.Data.DifficultyData.SpiritMovementSpeedModifier;
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
