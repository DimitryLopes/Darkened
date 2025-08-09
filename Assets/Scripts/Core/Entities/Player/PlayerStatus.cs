using System.Collections.Generic;

public class PlayerStatus
{
    public Dictionary<StatusKey, float> StatusDictionary { get; private set; } = new Dictionary<StatusKey, float>();
    public Dictionary<StatusKey, float> BaseStatusDictionary { get; private set; } = new Dictionary<StatusKey, float>();
    private Dictionary<string,StatusEffect> statusEffects = new();

    public float DepletedStaminaRegenSpeedMultiplier => StatusDictionary[StatusKey.DepletedStaminaRegenSpeedMultiplier];
    public float DepletedStaminaRegenCooldown => StatusDictionary[StatusKey.DepletedStaminaRegenCooldown];
    public float SprintingSpeedMultiplier => StatusDictionary[StatusKey.SprintingSpeedMultiplier];
    public float StaminaConsumptionSpeed => StatusDictionary[StatusKey.StaminaConsumptionSpeed];
    public float StaminaRegenSpeed => StatusDictionary[StatusKey.StaminaRegenSpeed];
    public float InteractionRange => StatusDictionary[StatusKey.InteractionRange];
    public float MovementSpeed => StatusDictionary[StatusKey.MovementSpeed];
    public float MaxStamina => StatusDictionary[StatusKey.MaxStamina];

    public void Setup(PlayerStatusSO baseStats)
    {
        foreach (Status status in baseStats.Statuses)
        {
            StatusDictionary.Add(status.Key, status.Value);
            BaseStatusDictionary.Add(status.Key, status.Value);
        }
    }

    public void ResetStatus()
    {
        foreach (var kvp in BaseStatusDictionary)
        {
            ResetStat(kvp.Key);
        }
    }

    public void ResetStat(StatusKey key)
    {
        StatusDictionary[key] = BaseStatusDictionary[key];
    }

    #region Status Effects
    public void ApplyStatusEffect(StatusEffect effect)
    {
        string effectKey = GetStatusEffectKey(effect);
        if(statusEffects.ContainsKey(effectKey))
        {
            statusEffects[effectKey].Reaply();
        }
        else
        {
            statusEffects.Add(effectKey, effect);
        }
    }

    public string GetStatusEffectKey(StatusEffect effect)
    {
        return string.Format(Constants.Player.STATUS_EFFECTS_KEY_FORMAT, effect.Stat, effect.Duration, effect.Value);
        
    }

    public void UpdateStatusEffects(float deltaTime)
    {
        foreach (var kvp in statusEffects)
        {
            if (kvp.Value.IsActive)
            {
                kvp.Value.Tick(deltaTime);
            }
        }
    }
    #endregion
}
