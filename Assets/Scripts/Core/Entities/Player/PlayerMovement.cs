using System.Collections;
using UnityEngine;
using Zenject;

public class PlayerMovement : PlayerAction
{
    [SerializeField] private Rigidbody2D rb;

    private SignalBus signalBus;
    private float currentStamina;
    private bool isSprinting;
    private bool isExhausted;
    private bool isMoving;

    public float CurrentStamina => currentStamina;

    public void SetUp(PlayerStatus status, SignalBus signalBus)
    {
        this.status = status;
        this.signalBus = signalBus;

        ResetMovement();
    }

    void Update()
    {
        HandleStaminaRegeneration();
        if (CanAct)
        {
            HandleSprinting();
            HandleMovement();
        }
    }

    private void HandleMovement()
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
            if (isSprinting)
            {
                movement *= status.SprintingSpeedMultiplier;
                ChangeStamina(-status.StaminaConsumptionSpeed * Time.deltaTime);
                if (currentStamina <= 0)
                {
                    StartCoroutine(Exaustion());
                }
            }
            float angle = Mathf.Atan2(-movement.x, movement.y) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
        }
        else
        {
            rb.velocity = Vector3.zero;
        }

        rb.velocity = movement;
    }

    private void HandleSprinting()
    {
        isSprinting = Input.GetKey(KeyCode.LeftShift) && !isExhausted;
    }

    private void HandleStaminaRegeneration()
    {
        if (isExhausted || (isMoving && isSprinting) || currentStamina >= status.MaxStamina) return;

        RegenerateStamina();
    }

    private void RegenerateStamina()
    {
        ChangeStamina(Mathf.Min(status.StaminaRegenSpeed * Time.deltaTime, status.MaxStamina));
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

        yield return new WaitForSeconds(status.DepletedStaminaRegenCooldown);


        while(currentStamina < status.MaxStamina)
        {
            RegenerateStamina();
            yield return null;
        }

        isExhausted = false;
    }
}