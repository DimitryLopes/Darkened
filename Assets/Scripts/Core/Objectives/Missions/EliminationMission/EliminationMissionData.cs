using UnityEngine;

[CreateAssetMenu(fileName = "Rename Me", menuName = "Scriptable Objects/Datas/Mission Datas/Elimination Mission Data")]
public class EliminationMissionData : MissionData
{
    [SerializeField]
    private int enemyHealth;
    public override int ProgressTarget => enemyHealth;
}
