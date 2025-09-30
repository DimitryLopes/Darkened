using UnityEngine;
using System.Collections.Generic;

public class TimeManager
{
    private const float BASE_TIME_SCALE = 1f;

    private List<object> pauseStack;


    public void Pause(object source)
    {
        if (pauseStack.Contains(source))
        {
            Debug.LogWarning($"{source} is already pausing the game.");
            return;
        }
        pauseStack.Add(source);
        Time.timeScale = 0f;
    }

    public void Resume(object source)
    {
        if (pauseStack.Contains(source))
        {
            pauseStack.Remove(source);
        }

        if(pauseStack.Count == 0)
        {
            Time.timeScale = BASE_TIME_SCALE;
        }
    }
}