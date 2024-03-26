using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
            foreach (MissionData missionData in Data.MissionGroups[i].Missions)
            {
                Mission newMission = new Mission(missionData, signalBus);
                newGroup.AddMission(newMission);
            }
            newGroup.SetActive(false);
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

    public void CompleteMission(Mission mission)
    {
        CurrentMissionGroup.CompleteMission(mission);
        if (CurrentMissionGroup.IsComplete)
        {
            CompleteMissionGroup();
        }
    }

    private void CompleteMissionGroup()
    {
        CurrentMissionGroup.SetActive(false);
        signalBus.Fire(new OnMissionGroupCompletedSignal(CurrentMissionGroup));

        currentGroupIndex++;
        if (currentGroupIndex < MissionGroups.Count)
        {
            CurrentMissionGroup.SetActive(true);
        }
        else
        {
            CompleteObjective();
        }
    }

    public void UpdateProgress(float amount)
    {
        Progress += amount;
        if (Progress >= 1)
        {
            CompleteObjective();
        }
    }

    public void CompleteObjective()
    {
        IsCompleted = true;
        signalBus.Fire(new OnGameCompletedSignal(this, true));
    }
}
