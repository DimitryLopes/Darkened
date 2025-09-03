using UnityEngine;

public interface IInteractable : IHighlightable
{
    bool CanInteract { get; }
    public void Interact();
    void OnInteract() { }
    Collider2D Collider { get; }
}
