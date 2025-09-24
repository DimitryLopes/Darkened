using UnityEngine;
using System;

public class TutorialAction : MonoBehaviour
{
    public TutorialActionData Data;
    public event Action OnCompleted;

    public void CheckForCompletion()
    {
        if (Input.GetKeyDown(Data.RequiredInput))
        {
            OnCompleted?.Invoke();
            //Destroy(gameObject); // Dest/rói o objeto após a conclusão
        }
    }
}