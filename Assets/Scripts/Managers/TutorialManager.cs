using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class TutorialManager : ITickable
{    
    private FloatingTextManager floatingTextManager;
    private PersistenceManager persistenceManager;
    private TutorialDatabase tutorialDatabase;
    private EntityManager entityManager;
    private SignalBus signalBus;

    public Tutorial CurrentTutorial { get; private set; }
    public Queue<Tutorial> tutorialQueue;

    [Inject]
    public TutorialManager(TutorialDatabase tutorialDatabase,FloatingTextManager floatingTextManager,
        EntityManager entityManager, PersistenceManager persistenceManager, SignalBus signalBus)
    {
        this.floatingTextManager = floatingTextManager;
        this.persistenceManager = persistenceManager;
        this.tutorialDatabase = tutorialDatabase;
        this.entityManager = entityManager;
        this.signalBus = signalBus;
    }

    private void Initialize()
    {
        tutorialQueue = new Queue<Tutorial>();
        List<TutorialData> tutorialDatas = tutorialDatabase.GetAllTutorials();
        foreach (var tutorialData in tutorialDatas)
        {
            if(tutorialData.SavedData.IsCompleted) continue;
            tutorialData.SetPersistenceKey();
            Tutorial tutorial = new Tutorial(tutorialData, OnTutorialActionCompleted);
            tutorialQueue.Enqueue(tutorial);
        }
    }

    private void StartTutorial(TutorialID id)
    {
        var data = tutorialDatabase.GetTutorial(id);

        Tutorial tutorial = new Tutorial(data, OnTutorialActionCompleted);
        CurrentTutorial = tutorial;
        tutorial.Start(floatingTextManager, entityManager.GetPlayer());
    }

    private void OnTutorialActionCompleted(Tutorial tutorial)
    {
        CurrentTutorial = null;
        tutorial.Data.SavedData.IsCompleted = true;
        persistenceManager.Save(tutorial.Data);
        signalBus.Fire(new OnTutorialCompletedSignal(tutorial));
    }

    public void Tick()
    {
        CurrentTutorial?.CheckForCompletion();
    }
}