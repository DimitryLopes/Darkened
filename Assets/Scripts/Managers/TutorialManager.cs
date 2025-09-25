using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class TutorialManager : ITickable
{    
    private FloatingTextManager floatingTextManager;
    private TutorialDatabase tutorialDatabase;
    private EntityManager entityManager;
    private SignalBus signalBus;

    public Tutorial CurrentTutorial { get; private set; }

    [Inject]
    public TutorialManager(TutorialDatabase tutorialDatabase,FloatingTextManager floatingTextManager,
        EntityManager entityManager, SignalBus signalBus)
    {
        this.floatingTextManager = floatingTextManager;
        this.tutorialDatabase = tutorialDatabase;
        this.entityManager = entityManager;
        this.signalBus = signalBus;
    }

    private void StartTutorial(TutorialID id)
    {
        var data = tutorialDatabase.GetTutorial(id);

        Tutorial tutorial = new Tutorial(data, OnTutorialActionCompleted);
        CurrentTutorial = tutorial;
    }

    private void OnTutorialActionCompleted(Tutorial tutorial)
    {
        CurrentTutorial = null;
        signalBus.Fire(new OnTutorialCompletedSignal(tutorial.Data.TutorialID));
    }

    public void Tick()
    {
        CurrentTutorial?.CheckForCompletion();
    }
}