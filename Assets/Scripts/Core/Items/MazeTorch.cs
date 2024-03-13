using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MazeTorch : Item
{
    [SerializeField]
    private Light2D light;

    public override void Interact()
    {
        if (canInteract)
        {
            light.enabled = true;
            canInteract = false;
        }
    }
}
