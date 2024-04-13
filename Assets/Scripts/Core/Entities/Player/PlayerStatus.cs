using UnityEngine;

[CreateAssetMenu(fileName = "Player Status", menuName = "Scriptable Objects/Player Status")]
public class PlayerStatus : ScriptableObject
{
    [SerializeField, Header("Movement")]
    private float rotationSpeed;
    [SerializeField]
    private float movementSpeed;
    [SerializeField]
    private float sprintingSpeedMultiplier;

    [SerializeField, Header("Stamina")]
    private float staminaRegenSpeed;
    [SerializeField]
    private float staminaConsumptionSpeed;
    [SerializeField]
    private float depletedStaminaRegenCooldown;
    [SerializeField]
    private float depletedStaminaRegenSpeedMultiplier;
    [SerializeField]
    private float maxStamina;

    [SerializeField, Header("Interaction")]
    private float interactionDistance;

    public float DepletedStaminaRegenCooldown => depletedStaminaRegenCooldown;
    public float DepletedStaminaRegenSpeedMultiplier => depletedStaminaRegenSpeedMultiplier;
    public float StaminaConsumptionSpeed => staminaConsumptionSpeed;
    public float InteractionRange => interactionDistance;
    public float StaminaRegenSpeed  => staminaRegenSpeed;
    public float SprintingSpeedMultiplier => sprintingSpeedMultiplier;
    public float MovementSpeed => movementSpeed;
    public float RotationSpeed => rotationSpeed;
    public float MaxStamina => maxStamina;
}
