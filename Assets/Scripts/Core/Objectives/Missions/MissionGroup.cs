using System.Collections.Generic;
using Zenject;

public class MissionGroup : IActivateable
{
    private SignalBus signalBus;

    public MissionGroupData Data { get; private set; }
    public List<IMission> Missions { get; private set; }
    public int CompletedMissions { get; private set; }
    public bool IsActive { get; set; }
    public bool IsComplete { get; set; }

    public MissionGroup(MissionGroupData data, SignalBus signalBus)
    {
        Data = data;
        CompletedMissions = 0;
        IsComplete = false;
        this.signalBus = signalBus;
    }

    public void AddMission(IMission mission)
    {
        if (Missions == null)
        {
            Missions = new List<IMission>();
        }

        Missions.Add(mission);
    }

    public void CompleteMission(IMission mission)
    {
        if (IsActive)
        {
            if (Missions.Contains(mission))
            {
                CompletedMissions++;
                if (CompletedMissions == Missions.Count)
                {
                    IsComplete = true;
                }
            }
        }
    }

    public void Activate()
    {
        IsActive = true;
        foreach (IMission mission in Missions)
        {
            mission.Activate();
        }
        signalBus.Fire(new OnMissionGroupStartedSignal(this));
    }

    public void Deactivate()
    {
        IsActive = false;
        foreach (IMission mission in Missions)
        {
            mission.Deactivate();
        }
    }
}
