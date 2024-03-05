public class Item : Activateable, IItem, IInteractable
{
    protected bool canInteract;
    public bool CanInteract => canInteract;

    public virtual void Interact()
    {
    }
}

