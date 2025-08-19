using UnityEngine;

public interface IStateUser
{
    Transform Transform { get; }
    public void Move(Vector3 target, bool isSprinting);
}
