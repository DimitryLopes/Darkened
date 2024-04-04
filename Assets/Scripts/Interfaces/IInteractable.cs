public interface IInteractable : IHighlightable
{
    bool CanInteract { get; }

    public void Interact();
}
