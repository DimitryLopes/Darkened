using Zenject;

public class ObjectiveManager 
{
    private readonly SignalBus signalBus;

    public Objective CurrentObjective { get; private set; }

    public ObjectiveManager(SignalBus signalBus)
    {
        this.signalBus = signalBus;

        signalBus.Subscribe<OnMissionCompletedSignal>(OnMissionCompleted);
        signalBus.Subscribe<OnMissionGroupCompletedSignal>(OnMissionGroupCompleted);
        signalBus.Subscribe<OnMazeLoadFinishSignal>(OnMazeLoadFinish);
    }

    public void StartObjective(ObjectiveData data)
    {
        Objective objective = new Objective(data, signalBus);
        SetObjective(objective);
    }

    public void SetObjective(Objective objective)
    {
        CurrentObjective = objective;
    }

    private void ActivateMissionGroup(MissionGroup missionGroup)
    {
        missionGroup.Activate();
    }
    
    public void AddMissionToItem(MissionItem item, ItemMission mission)
    {
        mission.Items.Add(item);
        item.DisableInteraction();
    }

    private void OnMissionCompleted(OnMissionCompletedSignal signal)
    {
        CurrentObjective.CompleteMission(signal.Mission);
    }

    private void OnMazeLoadFinish(OnMazeLoadFinishSignal signal)
    {
        ActivateMissionGroup(CurrentObjective.CurrentMissionGroup);
    }

    private void OnMissionGroupCompleted(OnMissionGroupCompletedSignal signal)
    {
    }

}
