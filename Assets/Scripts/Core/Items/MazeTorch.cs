using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MazeTorch : Item
{
    [SerializeField]
    private Light2D light;

    public bool isLit => light.enabled;

    public override void Interact()
    {
        if (CanInteract)
        {
            if(isLit)
            {
                DeactivateLights();
            }
            else
            {
                ActivateLights();
            }
        }
    }

    protected override void OnActivate()
    {
        base.OnActivate();
        DeactivateLights();
    }

    public void DeactivateLights()
    {
        light.enabled = false;
    }

    public void ActivateLights()
    {
        light.enabled = true;

    }
}
