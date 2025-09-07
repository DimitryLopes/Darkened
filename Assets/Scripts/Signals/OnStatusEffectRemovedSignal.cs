public class OnStatusEffectRemovedSignal
{
    public StatusEffect StatusEffect { get; private set; }

    public OnStatusEffectRemovedSignal(StatusEffect effect)
    {
        StatusEffect = effect;
    }
}
