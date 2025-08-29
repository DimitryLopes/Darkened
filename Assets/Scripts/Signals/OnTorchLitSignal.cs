using UnityEngine.Rendering.Universal;

public class OnTorchLitSignal
{
    public Light2D Light { get; private set; }
    public OnTorchLitSignal(Light2D light)
    {
        Light = light;
    }
}