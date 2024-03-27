using UnityEngine;

[CreateAssetMenu(fileName = "Rename me", menuName = "Scriptable Objects/Datas/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private float enhancedSpeedMultiplier;

    [SerializeField, Space]
    private float detectionRange;
    [SerializeField]
    private float enhancedDetectionRange;

    public float Speed => speed;
    public float SprintingSpeedMultiplier => enhancedSpeedMultiplier;
    public float DetectionRange => detectionRange;
    public float EnhancedDetectionRange => enhancedDetectionRange;
}
