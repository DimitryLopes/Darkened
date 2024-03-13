using UnityEngine;

[CreateAssetMenu(fileName = "Player Status", menuName = "Scriptable Objects/Player Status")]
public class PlayerStatus : ScriptableObject
{
    [SerializeField]
    private float rotationSpeed;
    [SerializeField]
    private float movementSpeed;
    [SerializeField]
    private float sprintingSpeed;

    [SerializeField, Header("Stamina")]
    private float staminaRegenSpeed;
    [SerializeField]
    private float staminaConsumptionSpeed;
    [SerializeField]
    private float depletedStaminaRegenCooldown;
    [SerializeField]
    private float maxStamina;

    public float DepletedStaminaRegenCooldown => depletedStaminaRegenCooldown;
    public float StaminaConsumptionSpeed => staminaConsumptionSpeed;
    public float StaminaRegenSpeed  => staminaRegenSpeed;
    public float SprintingSpeed => sprintingSpeed;
    public float MovementSpeed => movementSpeed;
    public float RotationSpeed => rotationSpeed;
    public float MaxStamina => maxStamina;
}
