using Zenject;

public class EliminationMission : Mission<EliminationMissionData>
{
    public EliminationMission(SignalBus signalBus) : base(signalBus)
    {
        signalBus.Subscribe<OnEnemyHitSignal>(OnMissionProgress);
    }

    public void OnMissionProgress(OnEnemyHitSignal Signal)
    {
        if (!IsActive) return;

        progress++;
        UpdateProgress();
    }
}
