using UnityEngine;
using Zenject;

public abstract class EnemyBase : MonoBehaviour
{
    [Inject]
    protected MazeManager mazeManager;

    //protected EnemyState currentState;

    //protected virtual void ChangeState(EnemyState state)
    //{
    //    currentState.Deactivate();
    //    currentState = state;
    //    currentState.Activate();
    //}

    //private void Update()
    //{
    //    currentState.HandleState();
    //}
}
