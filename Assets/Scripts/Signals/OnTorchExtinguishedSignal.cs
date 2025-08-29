using UnityEngine.Rendering.Universal;

public class OnTorchExtinguishedSignal
{
    public Light2D Light { get; private set; }
    public OnTorchExtinguishedSignal(Light2D light)
    {
        Light = light;
    }
}
