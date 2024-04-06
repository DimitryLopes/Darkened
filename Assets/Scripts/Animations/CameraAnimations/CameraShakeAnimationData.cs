using UnityEngine;

[CreateAssetMenu(fileName = "Rename Me", menuName = "Scriptable Objects/Datas/PostProcessing/Camera Shake Animation Data")]
public class CameraShakeAnimationData : PostProcessingData
{
    [SerializeField]
    private float amplitude;
    [SerializeField]
    private float frequency;

    public float Amplitude => amplitude;
    public float Frequency => frequency;
}
