using System;
using UnityEngine;

public static class ColorExtensions
{
    public static void TweenColorTo(this Color color, Color targetColor, float duration, GameObject gameObject, Action<Color> onUpdate, Action onComplete = null)
    {
        LeanTweenAnimationData data = new LeanTweenAnimationData(gameObject, 0, 1, duration, ChangeColors, onComplete);
        TweenUtils.DoTween(data);

        void ChangeColors(float floatito)
        {
            Color updatedColor = Color.Lerp(color, targetColor, floatito);
            onUpdate.Invoke(updatedColor);
            Debug.Log(updatedColor.a);
        }
    }
    
}


