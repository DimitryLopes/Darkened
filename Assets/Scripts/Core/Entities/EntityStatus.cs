using System.Collections.Generic;
using UnityEngine;

public class EntityStatus
{
    public Dictionary<StatusKey, float> StatusDictionary { get; private set; } = new Dictionary<StatusKey, float>();
    public Dictionary<StatusKey, float> BaseStatusDictionary { get; private set; } = new Dictionary<StatusKey, float>();
    protected Dictionary<string, StatusEffect> statusEffects = new();

    public float InteractionRange => StatusDictionary[StatusKey.InteractionRange];
    public float MovementSpeed => StatusDictionary[StatusKey.MovementSpeed]; 
    public float DepletedStaminaRegenSpeedMultiplier => StatusDictionary[StatusKey.DepletedStaminaRegenSpeedMultiplier];
    public float DepletedStaminaRegenCooldown => StatusDictionary[StatusKey.DepletedStaminaRegenCooldown];
    public float SprintingSpeedMultiplier => StatusDictionary[StatusKey.SprintingSpeedMultiplier];
    public float StaminaConsumptionSpeed => StatusDictionary[StatusKey.StaminaConsumptionSpeed];
    public float StaminaRegenSpeed => StatusDictionary[StatusKey.StaminaRegenSpeed];
    public float MaxStamina => StatusDictionary[StatusKey.MaxStamina];

    public float GetStat(StatusKey key)
    {
        if(StatusDictionary.ContainsKey(key))
        {
            return StatusDictionary[key];
        }
        else
        {
            Debug.LogWarning($"Stat {key} not found in StatusDictionary.");
            return 0f;
        }
    }

    public void Setup(EntityStatusSO baseStats)
    {
        foreach (Status status in baseStats.Statuses)
        {
            StatusDictionary.Add(status.Key, status.Value);
            BaseStatusDictionary.Add(status.Key, status.Value);
        }
    }

    public void ResetStatus()
    {
        ClearStatusEffects();
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
    public void ApplyStatusEffect(float multiplier, StatusKey status, float duration)
    {
        string effectKey = GetStatusEffectKey(multiplier, status, duration);
        if (statusEffects.ContainsKey(effectKey))
        {
            statusEffects[effectKey].Reaply();
        }
        else
        {
            StatusEffect effect = new StatusEffect(status, multiplier, duration, this);
            statusEffects.Add(effectKey, effect);
        }
    }

    private string GetStatusEffectKey(float multiplier, StatusKey status, float duration)
    {
        return string.Format(Constants.Player.STATUS_EFFECTS_KEY_FORMAT, status, duration, multiplier);
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

    public void ClearStatusEffects()
    {
        foreach (var kvp in statusEffects)
        {
            if (kvp.Value.IsActive)
            {
                kvp.Value.Deactivate();
            }
        }
    }
    #endregion
}
