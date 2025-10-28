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

        if (SpriteAnimator != null)
        {
            SetDefaultAnimationKey();
            PlayDefaultAnimation();
        }
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
        SpriteAnimator.PlayDefault();
        signalBus.Fire(new OnTorchExtinguishedSignal(torchlight));
    }

    public void ActivateLights()
    {
        torchlight.enabled = true;
        lightCollider.enabled = true;
        interactionCollider.enabled = true;
        string animationKey = string.Format(Constants.Items.TORCH_LIT_ANIMATION_KEY, alignedWith);
        SpriteAnimator.PlayAnimation(animationKey, null);
        signalBus.Fire(new OnTorchLitSignal(torchlight));
    }

    protected override void SetDefaultAnimationKey()
    {
        string orientation;
        if(alignedWith == Cardinal.North || alignedWith == Cardinal.South)
        {
            orientation = Constants.Orientations.VERTICAL;
        }
        else
        {
            orientation = Constants.Orientations.HORIZONTAL;
        }
        defaultAnimationKey = string.Format(Constants.Items.TORCH_UNLIT_ANIMATION_KEY, orientation);        
    }
}
