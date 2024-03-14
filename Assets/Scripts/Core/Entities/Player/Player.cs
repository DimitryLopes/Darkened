using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private PlayerMovement movement;
    [SerializeField]
    private PlayerStatus status;
    [SerializeField]
    private PlayerInteraction interaction;

    public void SetUp()
    {
        movement.SetUp(status);
    }

    public void ToggleActing(bool value)
    {
        movement.ToggleMovement(value);
        //interaction.ToggleInteraction(value);
    }
}
