using UnityEngine;

public class PlayerInteraction : PlayerAction
{
    [SerializeField]
    private LayerMask interactableLayer;

    public IInteractable currentInteractable;

    private void Update()
    {
        if (CanAct)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, status.InteractionRange, interactableLayer);

            Debug.DrawRay(transform.position, transform.up * status.InteractionRange, Color.yellow);
            IInteractable interactable = null;

            if (hit.collider != null)
            {
                interactable = hit.collider.GetComponent<IInteractable>();
            }

            ChangeInteractable(interactable);

            if (!Input.GetKeyDown(KeyCode.E)) return;

            TryInteract();

        }
    }

    private void ChangeInteractable(IInteractable interactable)
    {
        if (currentInteractable == interactable) return;

        currentInteractable?.RemoveHighlight();
        currentInteractable = interactable;
        interactable?.Highlight();
    }

    private void TryInteract()
    {
        if (currentInteractable == null) return;

        currentInteractable.Interact();
    }
}
