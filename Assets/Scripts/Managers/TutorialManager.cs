using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class TutorialManager : ITickable
{
    private Queue<TutorialActionData> tutorialQueue = new Queue<TutorialActionData>();
    private TutorialAction currentAction;

    [Inject]
    public TutorialManager(FloatingTextManager floatingTextManager, EntityManager entityManager)
    {
    }

    public void EnqueueTutorial(TutorialActionData tutorialData)
    {
        tutorialQueue.Enqueue(tutorialData);
        if (currentAction == null)
        {
            StartNextTutorial();
        }
    }

    private void StartNextTutorial()
    {
        if (tutorialQueue.Count == 0) return;

        TutorialActionData tutorialData = tutorialQueue.Dequeue();
        GameObject tutorialObject = new GameObject("TutorialAction");
        currentAction = tutorialObject.AddComponent<TutorialAction>();
        currentAction.Data = tutorialData;
        //ShowFloatingText(tutorialData.FloatingText);

        currentAction.OnCompleted += OnTutorialActionCompleted;
    }

    private void OnTutorialActionCompleted()
    {
        currentAction = null;
        StartNextTutorial();
    }

    public void Tick()
    {
        currentAction?.CheckForCompletion();
    }
}