using UnityEngine;

public class PostProcessingData : ScriptableObject
{
    [SerializeField]
    private float animationDuration = 2f;
    [SerializeField]
    private float priority;
    [SerializeField, Tooltip("whether or not the animation should remain in the screen after finishing")]
    private bool hideOnFinish;


    public float Priority => priority;
    public bool HideOnFinish => hideOnFinish;
    public float Duration => animationDuration;
}
