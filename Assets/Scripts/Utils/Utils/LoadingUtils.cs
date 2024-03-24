using UnityEngine;

public class LoadingUtils
{
    public static float GetProgress(float current, float total)
    {
        float progress = current / total;
        Debug.LogWarning(progress);
        return progress;
    }
}
