using UnityEngine;

public class LoadingUtils
{
    public static float GetProgress(float current, float total)
    {
        float progress = current / total;
        Mathf.Clamp(progress, 0, 1);
        return progress;
    }
}
