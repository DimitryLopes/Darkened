using UnityEngine;

[CreateAssetMenu(fileName = "Rename Me", menuName = "Scriptable Objects/Datas/PostProcessing/Vignette Animation Data")]
public class VignetteAnimationData : PostProcessingData
{
    [SerializeField]
    private float intensityTarget;
    [SerializeField]
    private float smoothnessTarget;

    public float Intensity => intensityTarget;
    public float Smoothness => smoothnessTarget;
}
