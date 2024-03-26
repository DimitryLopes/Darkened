using System.Collections.Generic;
using Zenject;

public class MissionGroup 
{
    private SignalBus signalBus;

    public MissionGroupData Data { get; private set; }
    public List<Mission> Missions { get; private set; }
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

    public void AddMission(Mission mission)
    {
        if (Missions == null)
        {
            Missions = new List<Mission>();
        }

        Missions.Add(mission);
    }

    public void CompleteMission(Mission mission)
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

    public void SetActive(bool value)
    {
        IsActive = value;

        foreach(Mission mission in Missions)
        {
            mission.SetActive(value);
        }

        if (value)
        {
            signalBus.Fire(new OnMissionGroupStartedSignal(this));
        }
    }
}
