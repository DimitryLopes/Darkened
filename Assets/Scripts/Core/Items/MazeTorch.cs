using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MazeTorch : Item
{
    [SerializeField]
    private Light2D torchlight;
    [SerializeField]
    private CircleCollider2D lightCollider;

    private MazeNode node;
    private Cardinal alignedWith;

    
    public bool isLit => torchlight.enabled;
    public MazeNode Node => node;
    public Cardinal AlignedWith => alignedWith;
    public override ItemType Type => ItemType.DefaultTorch;

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

    public void SetNode(MazeNode node, Cardinal cardinal)
    {
        this.node = node;
        alignedWith = cardinal;
    }

    public override void OnActivate()
    {
        base.OnActivate();
        DeactivateLights();
    }

    public void ToggleShadows(bool enabled)
    {
        torchlight.shadowsEnabled = enabled;
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
    }

    public void ActivateLights()
    {
        torchlight.enabled = true;
        lightCollider.enabled = true;
    }
}
