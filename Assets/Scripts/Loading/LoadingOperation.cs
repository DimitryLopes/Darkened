using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine;

public class LoadingOperation : ILoadable
{
    public UnityAction<LoadingStep> OnStepChanged { get; set; }
    public UnityAction OnLoadingComplete { get; set; }
    public float Progress { get; private set; }
    public bool IsComplete { get; private set; }

    private int currentIndex;
    private Coroutiner coroutiner;
    private List<LoadingStep> steps;

    public LoadingOperation(List<LoadingStep> steps, Coroutiner coroutiner,
        UnityAction<LoadingStep> OnStepChanged = null, UnityAction OnLoadingComplete = null)
    {
        this.OnStepChanged = OnStepChanged;
        this.OnLoadingComplete = OnLoadingComplete;
        this.coroutiner = coroutiner;
        this.steps = steps;
    }

    public void Load()
    {
        coroutiner.RunCoroutine(StartLoading());
    }


    private IEnumerator StartLoading()
    {
        currentIndex = 0;
        LoadingStep currentStep = steps[currentIndex];
        currentStep.Load();

        while(currentIndex < steps.Count - 1)
        {
            Progress = (steps[currentIndex].Progress + currentIndex) / steps.Count;
            Debug.Log("Loading Operation progress: " + Progress);
            if (steps[currentIndex].IsComplete)
            {
                currentIndex++;
                steps[currentIndex].Load();
                OnStepChanged?.Invoke(steps[currentIndex]);
            }
            yield return null;
        }
        Progress = 1;
        IsComplete = true;
        OnLoadingComplete?.Invoke();
    }
}

