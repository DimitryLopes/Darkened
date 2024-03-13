using UnityEngine;

[CreateAssetMenu(fileName = "Player Status", menuName = "Scriptable Objects/Player Status")]
public class PlayerStatus : ScriptableObject
{
    [SerializeField]
    private float movementSpeed;
    [SerializeField]
    private float sprintingSpeed;
    [SerializeField]
    private float staminaRegenSpeed;
    [SerializeField]
    private float staminaConsumptionSpeed;
    [SerializeField]
    private float staminaRegenCooldown;
    [SerializeField]
    private float depletedStaminaRegenCooldown;

    public float DepletedStaminaRegenCooldown => depletedStaminaRegenCooldown;
    public float StaminaRegenCooldown => staminaRegenCooldown;
    public float StaminaConsumptionSpeed => staminaConsumptionSpeed;
    public float StaminaRegenSpeed  => staminaRegenSpeed;
    public float SprintingSpeed => sprintingSpeed;
    public float MovementSpeed => movementSpeed;
}
