using Zenject;
using UnityEngine;

public class MazeExit : Item, IInteractable
{
    [Inject]
    private GameManager gameManager;

    [Inject]
    public void Create(GameManager gameManager)
    {
        this.gameManager = gameManager;
    }

    public bool CanInteract { get; private set; }

    public void Interact()
    {
        Debug.Log(gameManager);
    }

}


