using UnityEngine;

[CreateAssetMenu(fileName = "Rename Me", menuName = "Scriptable Objects/Settings/Difficulty")]
public class DifficultyData : ScriptableObject, IUISelectable
{
    [SerializeField]
    private string difficultyName;
    [SerializeField, Range(0, 2)]
    private float torchRadius;
    [SerializeField, Range(0, 1), Tooltip("Minimum toch percentage based on map size")]
    private float minTorchRatio;
    [SerializeField, Range(0, 1), Tooltip("Maximum toch percentage based on map size")]
    private float maxTorchRatio;

    public float MinimumTorchRatio => minTorchRatio;
    public float MaximumTorchRatio => maxTorchRatio;
    public float TorchRadius => torchRadius;
    public string Title => difficultyName;

    public SelectableType SelectableType => SelectableType.Difficulty;
}
