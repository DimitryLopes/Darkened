using System.Collections;
using UnityEngine;

public class PlayerMovement : PlayerAction
{
    [SerializeField] private Rigidbody2D rb;

    private float currentStamina;
    private bool isSprinting;
    private bool isExhausted;

    public override void SetUp(PlayerStatus status)
    {
        this.status = status;
        currentStamina = status.MaxStamina;
        isSprinting = false;
        isExhausted = false;
    }

    void Update()
    {
        if (CanAct)
        {
            HandleMovement();
            HandleSprinting();
        }
        HandleStaminaRegeneration();
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

        rb.velocity = movement * status.MovementSpeed;

        if (movement != Vector3.zero)
        {
            if (isSprinting)
            {
                movement *= status.SprintingSpeed;
                currentStamina -= status.StaminaConsumptionSpeed * Time.deltaTime;
                if (currentStamina <= 0)
                {
                    isExhausted = true;
                    currentStamina = 0;
                    isSprinting = false;
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
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isExhausted)
        {
            isSprinting = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift) || isExhausted)
        {
            isSprinting = false;
        }
    }

    private void HandleStaminaRegeneration()
    {
        if (!isSprinting && currentStamina < status.MaxStamina && !isExhausted)
        {
            if (currentStamina == 0)
            {
                StartCoroutine(StartStaminaRegeneration());
            }
            else
            {
                currentStamina = Mathf.Min(currentStamina + (status.MaxStamina / status.StaminaRegenSpeed) * Time.deltaTime, status.MaxStamina);
            }
        }
    }

    private IEnumerator StartStaminaRegeneration()
    {
        yield return new WaitForSeconds(status.DepletedStaminaRegenCooldown);
        isExhausted = false;
    }
}