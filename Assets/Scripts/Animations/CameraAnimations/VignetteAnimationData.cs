using UnityEngine;

[CreateAssetMenu(fileName = "Rename Me", menuName = "Scriptable Objects/Datas/PostProcessing/Vignette Animation Data")]
public class VignetteAnimationData : PostProcessingData
{
    [SerializeField]
    private AnimationCurve intensityCurve;
    [SerializeField]
    private AnimationCurve smoothnessCurve;

    public AnimationCurve IntensityCurve => intensityCurve;
    public AnimationCurve SmoothnessCurve => smoothnessCurve;
}
