using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Torch : Item
{
    [SerializeField]
    private Light2D torchlight;
    [SerializeField]
    private CircleCollider2D lightCollider;
    [SerializeField]
    private CircleCollider2D interactionCollider;


    private Cardinal alignedWith;
    
    public bool isLit => torchlight.enabled;
    public Cardinal AlignedWith => alignedWith;
    public override ItemType Type => ItemType.DefaultTorch;

    public override void Interact()
    {
        if (CanInteract)
        {
            if(isLit)
            {
                DeactivateLights();
                RemoveHighlight();
                signalBus.Fire(new OnTorchAbsorbedSignal(this));
                canInteract = false;
            }
            else
            {
                ActivateLights();
            }
        }
    }

    public void Setup(Cardinal cardinal)
    {
        alignedWith = cardinal;
    }

    public override void OnActivate()
    {
        base.OnActivate();
        canInteract = true;
        DeactivateLights();
    }

    public void SetLightRadius(float radius)
    {
        torchlight.pointLightOuterRadius = radius;
        lightCollider.radius = radius;
    }

    public void DeactivateLights()
    {
        torchlight.enabled = false;
        lightCollider.enabled = false;
        signalBus.Fire(new OnTorchExtinguishedSignal(torchlight));
    }

    public void ActivateLights()
    {
        torchlight.enabled = true;
        lightCollider.enabled = true;
        interactionCollider.enabled = true;
        signalBus.Fire(new OnTorchLitSignal(torchlight));
    }
}
