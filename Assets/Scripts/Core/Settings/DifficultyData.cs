using UnityEngine;

[CreateAssetMenu(fileName = "Rename Me", menuName = "Scriptable Objects/Settings/Difficulty")]
public class DifficultyData : ScriptableObject, IUISelectable
{
    [SerializeField]
    private DifficultyType difficulty;
    [SerializeField]
    private string difficultyName;
    [SerializeField, Range(0, 2)]
    private float torchRadius;
    [SerializeField, Range(0, 1), Tooltip("Minimum toch percentage based on map size")]
    private float minTorchRatio;
    [SerializeField, Range(0, 1), Tooltip("Maximum toch percentage based on map size")]
    private float maxTorchRatio;
    [SerializeField, Range(0, 1), Tooltip("Ration of torches that should be lit on start")]
    private float litTorchRatio;

    public float MinimumTorchRatio => minTorchRatio;
    public float LitTorchRatio => litTorchRatio;
    public float MaximumTorchRatio => maxTorchRatio;
    public float TorchRadius => torchRadius;
    public string Title => difficultyName;

    public DifficultyType DifficultyType => difficulty;

    public SelectableType SelectableType => SelectableType.Difficulty;
}
