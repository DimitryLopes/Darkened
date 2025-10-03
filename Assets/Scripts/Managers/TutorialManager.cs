using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Zenject;

public class TutorialManager : ITickable
{    
    private FloatingTextManager floatingTextManager;
    private PersistenceManager persistenceManager;
    private TutorialDatabase tutorialDatabase;
    private EntityManager entityManager;
    private Coroutiner coroutiner;
    private SignalBus signalBus;
    private bool isPlayingTutorial = false;

    private Queue<Tutorial> tutorialQueue;
    public Tutorial CurrentTutorial { get; private set; }

    [Inject]
    public TutorialManager(TutorialDatabase tutorialDatabase, FloatingTextManager floatingTextManager,
        EntityManager entityManager, PersistenceManager persistenceManager,
        TimeManager timeManager, SignalBus signalBus, Coroutiner coroutiner)
    {
        this.floatingTextManager = floatingTextManager;
        this.persistenceManager = persistenceManager;
        this.tutorialDatabase = tutorialDatabase;
        this.entityManager = entityManager;
        this.signalBus = signalBus;
        this.coroutiner = coroutiner;
        Initialize();
    }

    private void Initialize()
    {
        tutorialQueue = new Queue<Tutorial>();
        List<TutorialData> tutorialDatas = tutorialDatabase.GetAllTutorials();
        foreach (var tutorialData in tutorialDatas)
        {
            if(tutorialData.SavedData.IsCompleted) continue;
            Tutorial tutorial = new Tutorial(tutorialData, OnTutorialActionCompleted);
            tutorialQueue.Enqueue(tutorial);
        }
        isPlayingTutorial = false;
        ListenToNextTutorial();
    }

    private void ListenToNextTutorial()
    {
        if(tutorialQueue.Count == 0 || CurrentTutorial != null) return;

        CurrentTutorial = tutorialQueue.Peek();
        TutorialTriggerType triggerType = CurrentTutorial.Data.TriggerData.TriggerType;
        switch (triggerType)
        {
            case TutorialTriggerType.Level:
                signalBus.Subscribe<OnMazeLoadFinishSignal>(OnLevelLoaded);
                break;
            default:
                signalBus.Subscribe<OnPlayerInteractableChangedSignal>(OnPlayerInteractableChanged);
                break;
        }
    }

    private void StartTutorial(Tutorial tutorial)
    {
        tutorial.Start(floatingTextManager, entityManager.GetPlayer());
        isPlayingTutorial = true;
        signalBus.Fire(new OnTutorialStartedSignal(tutorial));
    }

    private void OnTutorialActionCompleted(Tutorial tutorial)
    {
        isPlayingTutorial = false;
        CurrentTutorial = null;
        tutorial.Data.SavedData.IsCompleted = true;
        persistenceManager.Save(tutorial.Data);
        signalBus.Fire(new OnTutorialCompletedSignal(tutorial));
    }

    public void Tick()
    {
        if (!isPlayingTutorial) return;
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
                coroutiner.StartCoroutine(WaitForTutorial());
            }
        }
        signalBus.Unsubscribe<OnMazeLoadFinishSignal>(OnLevelLoaded);
    }

    private IEnumerator WaitForTutorial()
    {
        yield return new WaitForSeconds(2);
        StartTutorial(CurrentTutorial);
    }

    private void OnPlayerInteractableChanged(OnPlayerInteractableChangedSignal signal)
    {
        if(CurrentTutorial == null || CurrentTutorial.Data.TriggerData.TriggerType != TutorialTriggerType.Interaction) return;
        if(signal.Item == null) return;

        if(signal.Item.Type == CurrentTutorial.Data.TriggerData.ItemType)
        {
            StartTutorial(CurrentTutorial);
        }
        signalBus.Unsubscribe<OnPlayerInteractableChangedSignal>(OnPlayerInteractableChanged);
    }
    #endregion
}