using UnityEngine;

public class PostProcessingData : ScriptableObject
{
    [SerializeField]
    private float animationDuration = 2f;
    [SerializeField]
    private float priority;
    [SerializeField]
    private bool hideOnFinish;


    public float Priority => priority;
    public bool HideOnFinish => hideOnFinish;
    public float Duration => animationDuration;
    public bool Indefinite => animationDuration > 0;
}
