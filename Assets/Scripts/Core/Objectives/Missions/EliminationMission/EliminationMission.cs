using Zenject;

public class EliminationMission : Mission<EliminationMissionData>
{
    public EliminationMission(SignalBus signalBus) : base(signalBus)
    {
        signalBus.Subscribe<OnEnemyHitSignal>(OnMissionProgress);
    }

    public void OnMissionProgress(OnEnemyHitSignal signal)
    {
        if (!IsActive || signal.Item.Type != ItemType.Arrow) return;

        progress++;
        UpdateProgress();
    }
}
