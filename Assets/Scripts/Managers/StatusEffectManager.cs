using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class StatusEffectManager
{
    private VisualStatusEffectFactory effectFactory;
    private StatusEffectVisualDataBase visualDataBase;
    private List<EffectInstance> activeEffects = new();

    public StatusEffectManager(VisualStatusEffectFactory effectFactory, SignalBus signalBus,
        StatusEffectVisualDataBase effectVisualDataBase)
    {
        this.effectFactory = effectFactory;
        visualDataBase = effectVisualDataBase;
        signalBus.Subscribe<OnStatusEffectAppliedSignal>(OnStatusEffectApplied);
        signalBus.Subscribe<OnStatusEffectRemovedSignal>(OnStatusEffectRemoved);
    }

    public void OnStatusEffectApplied(OnStatusEffectAppliedSignal signal)
    {
        string key = GetStatusEffectKey(signal.StatusEffect.Multiplier, signal.StatusEffect.Stat, signal.StatusEffect.MaxDuration);
        bool isPositive = signal.StatusEffect.Multiplier > 1f;
        VisualEntityStatusEffect effectInstance = GetAvailableVisualEffect(key);
        StatusEffectVisualInfo visualInfo = visualDataBase.GetVisualInfo(signal.StatusEffect.Stat, isPositive);
        effectInstance.SetEffect(signal.TargetRenderer, isPositive ? visualInfo.PositiveMaterial : visualInfo.NegativeMaterial);
    }

    public void OnStatusEffectRemoved(OnStatusEffectRemovedSignal signal)
    {
        string effectKey = GetStatusEffectKey(signal.StatusEffect.Multiplier, signal.StatusEffect.Stat, signal.StatusEffect.MaxDuration);
        for (int i = 0; i < activeEffects.Count; i++)
        {
            var effect = activeEffects[i];
            if(effect.Key == effectKey)
            {
                effect.Instance.Deactivate();
                return;
            }
        }
        Debug.LogWarning($"Tried to remove status effect with key {effectKey} but it was not found among active effects.");
    }

    public VisualEntityStatusEffect GetAvailableVisualEffect(string key)
    {
        for (int i = 0; i < activeEffects.Count; i++)
        {
            var effect = activeEffects[i];
            effect.Key = key;
            effect.Instance.Activate();
            return effect.Instance;

        }
        VisualEntityStatusEffect newEffect = effectFactory.Create();
        activeEffects.Add(new EffectInstance(newEffect, key));
        newEffect.Activate();
        return newEffect;
    }

    public static string GetStatusEffectKey(float multiplier, StatusKey status, float duration)
    {
        return string.Format(Constants.Entities.STATUS_EFFECTS_KEY_FORMAT, status, duration, multiplier);
    }

    private struct EffectInstance
    {
        public VisualEntityStatusEffect Instance;
        public string Key;

        public EffectInstance(VisualEntityStatusEffect instance, string key)
        {
            Instance = instance;
            Key = key;
        }
    }
}
