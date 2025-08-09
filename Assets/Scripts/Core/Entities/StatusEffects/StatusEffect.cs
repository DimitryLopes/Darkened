public class StatusEffect 
{
    public StatusKey Stat { get; private set; }
    public float Value { get; private set; }
    public float Duration { get; private set; }
    public bool IsActive { get; private set; }
    public float MaxDuration { get; private set; }

    public StatusEffect(StatusKey key, float value, float duration)
    {
        Stat = key;
        Value = value;
        Duration = duration;
        MaxDuration = duration;
        IsActive = true;
    }

    public void Tick(float deltaTime)
    {
        Duration -= deltaTime;
        if (Duration <= 0)
        {
            IsActive = false;
        }
    }

    public void Reaply()
    {
        Duration = MaxDuration;
        IsActive = true;
    }
}
