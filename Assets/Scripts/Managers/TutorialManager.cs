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

    public Queue<Tutorial> tutorialQueue;
    public Tutorial CurrentTutorial { get; private set; }

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
        ListenToNextTutorial();
    }

    private void ListenToNextTutorial()
    {
        if(tutorialQueue.Count == 0 || CurrentTutorial != null) return;
        TutorialTriggerType triggerType = tutorialQueue.Peek().Data.TriggerData.TriggerType;
        switch (triggerType)
        {
            case TutorialTriggerType.Level:
                StartTutorial(tutorialQueue.Peek());
                break;
            default:
                StartTutorial(tutorialQueue.Peek());
                break;
        }
    }

    private void StartTutorial(Tutorial tutorial)
    {
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

    #region Tutorial Listeners
    private void OnLevelLoaded(OnMazeLoadFinishSignal signal)
    {
        if(CurrentTutorial == null || CurrentTutorial.Data.TriggerData.TriggerType != TutorialTriggerType.Level) return;

        if (signal.Maze.Data is PresetLevelData data)
        {
            if(data.ID == CurrentTutorial?.Data.TriggerData.LevelID)
            {
                StartTutorial(CurrentTutorial);
            }
        }
    }

    private void OnPlayerInteractableChanged(OnPlayerInteractableChangedSignal signal)
    {
        if(CurrentTutorial == null || CurrentTutorial.Data.TriggerData.TriggerType != TutorialTriggerType.Interaction) return;
        if(signal.Item == null) return;

        if(signal.Item.Type == CurrentTutorial.Data.TriggerData.ItemType)
        {
            StartTutorial(CurrentTutorial);
        }
    }
    #endregion
}