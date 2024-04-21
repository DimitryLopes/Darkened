using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MazeTorch : Item
{
    [SerializeField]
    private Light2D torchlight;

    public bool isLit => torchlight.enabled;

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

    public override void OnActivate()
    {
        base.OnActivate();
        DeactivateLights();
    }

    public void SetLightRadius(float radius)
    {
        torchlight.pointLightOuterRadius = radius;
    }

    public void DeactivateLights()
    {
        torchlight.enabled = false;
    }

    public void ActivateLights()
    {
        torchlight.enabled = true;
    }
}
