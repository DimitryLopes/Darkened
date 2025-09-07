using UnityEngine;

public class OnStatusEffectAppliedSignal
{
    public StatusEffect StatusEffect { get; private set; }
    public SpriteRenderer TargetRenderer { get; private set; }

    public OnStatusEffectAppliedSignal(StatusEffect effect, SpriteRenderer baseRenderer)
    {
        StatusEffect = effect;
        TargetRenderer = baseRenderer;
    }
}
