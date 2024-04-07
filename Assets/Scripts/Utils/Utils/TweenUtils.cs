using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenUtils
{
    public static void CancelTween(GameObject gameObject, bool finishCurrent)
    {
        bool isTweening = gameObject.LeanIsTweening();
        if (!isTweening) return;

        LeanTween.cancel(gameObject, finishCurrent);
    }

    public static void DoTween(LeanTweenAnimationData data, bool finishCurrent = true)
    {
        CancelTween(data.GameObject, finishCurrent);
        LeanTween.value(data.GameObject, data.From, data.To, data.Duration).setOnUpdate(data.OnUpdate).setOnComplete(data.OnComplete);
    }
}

public struct LeanTweenAnimationData
{
    public LeanTweenAnimationData(GameObject gameObject, float from, float to, float duration, Action<float> onUpdate, Action onComplete = null)
    {
        GameObject = gameObject;
        From = from;
        To = to;
        Duration = duration;
        OnUpdate = onUpdate;
        OnComplete = onComplete;
    }

    public GameObject GameObject { get; private set; }
    public float From { get; private set; }
    public float To { get; private set; }
    public float Duration { get; private set; }
    public Action<float> OnUpdate { get; private set; }
    public Action OnComplete { get; private set; }

}
