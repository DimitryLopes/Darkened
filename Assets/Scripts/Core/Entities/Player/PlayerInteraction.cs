using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : PlayerAction
{

    private void Update()
    {
        if (CanAct)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                TryInteract();
            }
        }
    }

    private void TryInteract()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, status.InteractionRange);

        Debug.DrawRay(transform.position, transform.up * status.InteractionRange, Color.yellow, 1f);
        if (hit.collider != null)
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact();
            }
        }
    }
}
