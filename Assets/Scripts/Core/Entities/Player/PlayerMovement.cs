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

    public float CurrentStamina => currentStamina;

    public void SetUp(PlayerStatus status, Joystick joystick, UIToggleButton sprintButton, SignalBus signalBus)
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
            SetSprinting();
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
        bool isMoving = movement.magnitude >= 0.1f;

        if (isMoving)
        {
            if (isSprinting)
            {
                movement *= status.SprintingSpeedMultiplier;
                ChangeStamina(-status.StaminaConsumptionSpeed * Time.deltaTime);
                if (currentStamina <= 0)
                {
                    StartCoroutine(Exaustion());
                }
            }
            HandleRotation(movement);
        }

        rb.velocity = movement;
    }

    private void HandlePCMovement()
    {
        Vector3 movement = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            movement += Vector3.up;
        }
        if (Input.GetKey(KeyCode.A)) 
        {
            movement -= Vector3.right;
        }
        if (Input.GetKey(KeyCode.S))
        {
            movement -= Vector3.up;
        }
        if (Input.GetKey(KeyCode.D))
        {
            movement += Vector3.right;
        }

        isMoving = movement != Vector3.zero;
        movement *= status.MovementSpeed;

        if (isMoving)
        {
            movement = HandleSprinting(movement);
            HandleRotation(movement);
        }
        else
        {
            rb.velocity = Vector3.zero;
        }

        rb.velocity = movement;
    }

    private Vector3 HandleSprinting(Vector3 movement)
    {
        if (isSprinting)
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

    private void SetSprinting()
    {
        isSprinting = Input.GetKey(KeyCode.LeftShift) || sprintButton.IsToggled && !isExhausted;
    }

    private void HandleStaminaRegeneration()
    {
        if (isExhausted || (isMoving && isSprinting) || currentStamina >= status.MaxStamina) return;

        RegenerateStamina();
    }

    private void RegenerateStamina(float speedMultiplier = 1f)
    {
        ChangeStamina(Mathf.Min(status.StaminaRegenSpeed * Time.deltaTime * speedMultiplier, status.MaxStamina));
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


        while(currentStamina < status.MaxStamina)
        {
            RegenerateStamina(status.DepletedStaminaRegenSpeedMultiplier);
            yield return null;
        }

        isExhausted = false;
        signalBus.Fire<OnPlayerExaustedRecoveredSignal>();
    }
}