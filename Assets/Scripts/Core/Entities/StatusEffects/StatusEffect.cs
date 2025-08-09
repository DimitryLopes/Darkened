using System;

public class StatusEffect 
{
    public StatusKey Stat { get; private set; }
    public float Multiplier { get; private set; }
    public float Duration { get; private set; }
    public bool IsActive { get; private set; }
    public float MaxDuration { get; private set; }
    private PlayerStatus Status { get; set; }

    public StatusEffect(StatusKey key, float value, float duration, PlayerStatus status)
    {
        Stat = key;
        Multiplier = value;
        Duration = duration;
        MaxDuration = duration;
        Status = status;
    }

    public void Tick(float deltaTime)
    {
        Duration -= deltaTime;
        if (Duration <= 0)
        {
            IsActive = false;
            Deactivate();
        }
    }

    public void Reaply()
    {
        Duration = MaxDuration;
        Activate();
    }

    public void Deactivate()
    {
        IsActive = false;
        Status.StatusDictionary[Stat] /= Multiplier;
    }

    public void Activate()
    {
        IsActive = true;
        Status.StatusDictionary[Stat] *= Multiplier;
    }
}
[Serializable]
public struct StatusEffectData
{
    public StatusKey Stat;
    public float Multiplier;
    public float Duration;
}
