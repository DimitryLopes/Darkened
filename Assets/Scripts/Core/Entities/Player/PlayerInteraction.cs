using UnityEngine;
using System.Collections.Generic;
using Zenject;

public class PlayerInteraction : PlayerAction
{
    [SerializeField]
    private CircleCollider2D interactionCollider;
    [SerializeField]
    private LayerMask interactableLayer;
    [SerializeField]
    private LayerMask obstructionLayer;

    public IInteractable currentInteractable;
    private SignalBus signalBus;
    private List<IInteractable> interactablesInRange = new List<IInteractable>();


    public void SetUp(EntityStatus status, SignalBus signalBus)
    {
        this.status = status;
        this.signalBus = signalBus;
        interactionCollider.radius = status.InteractionRange;
        signalBus.Subscribe<OnInteractionButtonClickedSignal>(TryInteract);
    }

    private void Update()
    {
        if (CanAct)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                signalBus.Fire(new OnPlayerTryToUseItemSignal());
            }

            if (interactablesInRange.Count == 0) return;

            IInteractable interactable = FindClosestInteractable();

            if (interactable == null) return;

            ChangeInteractable(interactable);

            if (!Input.GetKeyDown(KeyCode.E)) return;

            TryInteract();

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var interactable = collision.gameObject.GetComponent<IInteractable>();

        if (interactable == null) return;

        interactablesInRange.Remove(interactable);

        if (interactablesInRange.Count > 0) return;

        ChangeInteractable(null);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var interactable = collision.gameObject.GetComponent<IInteractable>();
        if (interactable == null) return;
        interactablesInRange.Add(interactable);
    }

    private IInteractable FindClosestInteractable()
    {
        IInteractable closestInteractable = null;
        float closestDistance = Mathf.Infinity;
        foreach (IInteractable interactable in interactablesInRange)
        {
            if (IsObstructed(interactable.Collider)) continue;

            float distance = Vector3.Distance(transform.position, interactable.Collider.bounds.center);
            if (distance < closestDistance)
            {
                closestInteractable = interactable;
                closestDistance = distance;
            }
        }
        return closestInteractable;
    }

    private bool IsObstructed(Collider2D target)
    {
        float distance = Vector3.Distance(transform.position, target.bounds.center);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, (target.bounds.center - transform.position).normalized, distance, obstructionLayer);
        return hit.collider != null;
    }
    private void OnDrawGizmos()
    {
        if (currentInteractable != null)
        {
            Vector3 direction = (currentInteractable.Collider.bounds.center - transform.position).normalized;
            Gizmos.color = IsObstructed(currentInteractable.Collider) ? Color.red : Color.green;
            Gizmos.DrawRay(transform.position, direction * status.InteractionRange);
        }
    }

    private void ChangeInteractable(IInteractable interactable)
    {
        if (currentInteractable == interactable) return;

        currentInteractable?.RemoveHighlight();
        currentInteractable = interactable;
        interactable?.Highlight();
        signalBus.Fire(new OnPlayerInteractableChangedSignal(currentInteractable as Item));
    }

    private void TryInteract()
    {
        if (currentInteractable == null) return;

        currentInteractable.Interact();
    }
}
