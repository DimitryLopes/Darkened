using System.Collections;
using UnityEngine;
using Zenject;

public class PlayerMovement : PlayerAction
{
    [SerializeField] private Rigidbody2D rb;

    private SignalBus signalBus;
    private Joystick joystick;
    private UIToggleButton sprintButton;
    private float currentStamina;
    private bool isSprinting;
    private bool isExhausted;
    private bool isMoving;
    private bool isRegeneratingAfterExaustion;

    public float CurrentStamina => currentStamina;
    private Vector3 movement;

    public void SetUp(EntityStatus status, Joystick joystick, UIToggleButton sprintButton, SignalBus signalBus)
    {
        this.status = status;
        this.signalBus = signalBus;
        this.joystick = joystick;
        this.sprintButton = sprintButton;
        ResetMovement();
    }

    void Update()
    {
        HandleStaminaRegeneration();
        if (CanAct)
        {
            SetIsSprinting();
            if (GameManager.IsOnPhone)
            {
                HandlePhoneMovement();
            }
            else
            {
                HandlePCMovement();
            }
        }
    }

    private void HandlePhoneMovement()
    {
        Vector3 movement = joystick.Direction.normalized;
        isMoving = movement.magnitude >= 0.1f;

        if (isMoving)
        {
            movement = HandleSprinting(movement);
            HandleRotation(movement);
        }

        rb.velocity = movement;
    }

    public override void ToggleActing(bool value)
    {
        base.ToggleActing(value);
        StopMovement();
    }

    private void StopMovement()
    {
        rb.velocity = Vector3.zero;
    }

    private void HandlePCMovement()
    {
        movement = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) movement.y += 1;
        if (Input.GetKey(KeyCode.S)) movement.y -= 1;
        if (Input.GetKey(KeyCode.D)) movement.x += 1;
        if (Input.GetKey(KeyCode.A)) movement.x -= 1;

        isMoving = movement.magnitude >= 0.1f;
        movement *= status.MovementSpeed;

        if (isMoving)
        {
            movement = HandleSprinting(movement);
            HandleRotation(movement);
        }
        else
        {
            rb.velocity = Vector3.zero;
            return;
        }

        rb.velocity = movement;
    }

    private Vector3 HandleSprinting(Vector3 movement)
    {
        if (isSprinting && !isExhausted)
        {
            movement *= status.SprintingSpeedMultiplier;
            ChangeStamina(-status.StaminaConsumptionSpeed * Time.deltaTime);
            if (currentStamina <= 0)
            {
                StartCoroutine(Exaustion());
            }
        }

        return movement;
    }

    private void HandleRotation(Vector3 movement)
    {
        float angle = Mathf.Atan2(-movement.x, movement.y) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
    }

    private void SetIsSprinting()
    {
        isSprinting = Input.GetKey(KeyCode.LeftShift) || sprintButton.IsToggled;
    }

    private void HandleStaminaRegeneration()
    {
        if (isRegeneratingAfterExaustion)
        {
            RegenerateStamina(status.DepletedStaminaRegenSpeedMultiplier);
            return;
        }

        if (isExhausted || (isMoving && isSprinting) || currentStamina >= status.MaxStamina) return;

        RegenerateStamina();
    }

    private void RegenerateStamina(float speedMultiplier = 1f)
    {
        float regenAmount = Mathf.Min(status.StaminaRegenSpeed * Time.deltaTime * speedMultiplier, status.MaxStamina);
        ChangeStamina(regenAmount);
    }

    private void ChangeStamina(float amount)
    {
        currentStamina += amount;
        signalBus.Fire(new OnPlayerStaminaChangedSignal());
    }

    public void ResetMovement()
    {
        currentStamina = status.MaxStamina;
        isSprinting = false;
        isExhausted = false;
    }

    private IEnumerator Exaustion()
    {
        isExhausted = true;
        isSprinting = false;
        currentStamina = 0;
        signalBus.Fire<OnPlayerStaminaExaustedSignal>();

        yield return new WaitForSeconds(status.DepletedStaminaRegenCooldown);

        isRegeneratingAfterExaustion = true;

        while (currentStamina < status.MaxStamina)
        {
            yield return null;
        }

        isRegeneratingAfterExaustion = false;
        isExhausted = false;
        signalBus.Fire<OnPlayerExaustedRecoveredSignal>();
    }
}