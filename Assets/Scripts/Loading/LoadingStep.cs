using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class LoadingStep : ILoadable 
{
    private Coroutiner coroutiner;
    private UnityAction onFinishAction;

    public IEnumerator<float> Step { get; private set; }
    //public TrackedEnumerator<float> TrackedStep { get; private set; }
    public float Progress { get; private set; }
    public bool IsComplete { get; private set; }
    public string Name { get; private set; }

    public LoadingStep(IEnumerator<float> enumerator, Coroutiner coroutiner, string name, 
        UnityAction OnFinishAction = null)
    {
        //TrackedEnumerator<float> progressEnumerator = new TrackedEnumerator<float>(enumerator, totalSteps);
        //TrackedStep = progressEnumerator;

        this.coroutiner = coroutiner;
        Step = enumerator;
        Name = name;
        onFinishAction += OnFinishAction;
    }

    public void Load()
    {
        Progress = 0;
        IsComplete = false;
        if(Step != null)
        {
            coroutiner.RunCoroutine(LoadStep());
        }
    }

    private IEnumerator LoadStep()
    {
        coroutiner.RunCoroutine(Step);
        while (Step.MoveNext())
        {
            Progress = Step.Current;
            yield return null;
        }

        Finish();
    }

    private void Finish()
    {
        IsComplete = true;
        Progress = 1;
        onFinishAction?.Invoke();
    }
}
