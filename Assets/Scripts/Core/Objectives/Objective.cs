using System.Collections.Generic;
using Zenject;

public class Objective 
{
    private ObjectiveData data;
    private SignalBus signalBus;
    private int currentGroupIndex;

    public ObjectiveData Data => data;
    public bool IsCompleted { get; private set; }
    public float Progress { get; private set; }
    public List<MissionGroup> MissionGroups { get; private set; }
    public MissionGroup CurrentMissionGroup => MissionGroups[currentGroupIndex];

    public Objective (ObjectiveData data, SignalBus signalBus)
    {
        SetUp(data, signalBus);

        for (int i = 0; i < Data.MissionGroups.Count; i++)
        {
            MissionGroup newGroup = new MissionGroup(Data.MissionGroups[i], signalBus);
            foreach (MissionData missionData in Data.MissionGroups[i].Datas)
            {
                if (missionData is ItemMissionData itemMissionData)
                {
                    ItemMission itemMission = new ItemMission(signalBus);
                    itemMission.SetUp(itemMissionData);
                    newGroup.AddMission(itemMission);
                }
                else if (missionData is EliminationMissionData eliminationMissionData)
                {
                    EliminationMission eliminationMissions = new EliminationMission(signalBus);
                    eliminationMissions.SetUp(eliminationMissionData);
                    newGroup.AddMission(eliminationMissions);
                }
            }
            newGroup.Deactivate();
            MissionGroups.Add(newGroup);
        }
    }

    private void SetUp(ObjectiveData data, SignalBus signalBus)
    {
        this.data = data;
        this.signalBus = signalBus;
        MissionGroups = new List<MissionGroup>();
        currentGroupIndex = 0;
        IsCompleted = false;
    }

    public void CompleteMission(IMission mission)
    {
        CurrentMissionGroup.CompleteMission(mission);
        if (CurrentMissionGroup.IsComplete)
        {
            CompleteMissionGroup();
        }
    }

    private void CompleteMissionGroup()
    {
        CurrentMissionGroup.Deactivate();
        signalBus.Fire(new OnMissionGroupCompletedSignal(CurrentMissionGroup));

        currentGroupIndex++;
        if (currentGroupIndex < MissionGroups.Count)
        {
            CurrentMissionGroup.Activate();
        }
        else
        {
            CompleteObjective();
        }
    }

    public void CompleteObjective()
    {
        IsCompleted = true;
        DeactivateAllMissionGroups();
        signalBus.Fire(new OnGameCompletedSignal(true));
    }

    public void CompleteCurrentMissionGroup()
    {
        CurrentMissionGroup.ForceCompleteAllMissions();
        CompleteMissionGroup();
    }

    private void DeactivateAllMissionGroups()
    {
        foreach(MissionGroup group in MissionGroups)
        {
            group.Deactivate();
        }
    }
}
