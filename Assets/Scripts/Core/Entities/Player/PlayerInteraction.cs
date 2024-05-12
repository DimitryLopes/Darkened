using UnityEngine;
using Zenject;

public class PlayerInteraction : PlayerAction
{
    [SerializeField]
    private LayerMask interactableLayer;
    [SerializeField]
    private LayerMask obstructionLayer;

    public IInteractable currentInteractable;

    private SignalBus signalBus;

    public void SetUp(PlayerStatus status, SignalBus signalBus)
    {
        this.status = status;
        this.signalBus = signalBus;

        signalBus.Subscribe<OnInteractionButtonClickedSignal>(TryInteract);
    }

    private void Update()
    {
        if (CanAct)
        {

            if (Input.GetKeyDown(KeyCode.Q))
            {
                signalBus.Fire(new OnPlayerItemUsedSignal());
            }

            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, status.InteractionRange, interactableLayer);

            if (colliders.Length == 0)
            {
                ChangeInteractable(null);
            }

            Collider2D closestCollider = FindClosestCollider(colliders);

            if (!closestCollider) return; //if it's null

            IInteractable interactable = closestCollider.GetComponent<IInteractable>();
            ChangeInteractable(interactable);

            if (!Input.GetKeyDown(KeyCode.E)) return;

            TryInteract();

        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        if (status == null) return;
        Gizmos.DrawWireSphere(transform.position, status.InteractionRange);
    }

    private Collider2D FindClosestCollider(Collider2D[] colliders)
    {
        Collider2D closestCollider = null;
        float closestDistance = Mathf.Infinity;
        foreach (Collider2D collider in colliders)
        {
            if (IsObstructed(collider.gameObject)) continue;

            float distance = Vector3.Distance(transform.position, collider.transform.position);
            if (distance < closestDistance)
            {
                closestCollider = collider;
                closestDistance = distance;
            }
        }
        return closestCollider;
    }

    private bool IsObstructed(GameObject target)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, (target.transform.position - transform.position).normalized, status.InteractionRange, obstructionLayer);
        return hit.collider != null;
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
