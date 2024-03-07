using Zenject;

public class ObjectiveManager 
{
    private readonly SignalBus signalBus;

    public Objective CurrentObjective { get; private set; }

    [Inject]
    public ObjectiveManager(SignalBus signalBus)
    {
        this.signalBus = signalBus;

        signalBus.Subscribe<OnMissionCompletedSignal>(OnMissionCompleted);
    }

    public void SetObjective(Objective objective)
    {
        CurrentObjective = objective;
        objective.StartObjective(signalBus);
    }

    private void ActivateMission(Mission mission)
    {

    }
    
    public void AddMissionToItem(MissionItem item, Mission mission)
    {
        item.SetMission(mission);
    }

    private void OnMissionCompleted(OnMissionCompletedSignal signal)
    {
        CurrentObjective.CompleteMission(signal.Mission);
    }

}
