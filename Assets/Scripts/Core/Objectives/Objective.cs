using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Objective : ScriptableObject, IUISelectable
{
    [SerializeField]
    private string title;
    [SerializeField]
    private string description;
    [SerializeField]
    private string victoryMessage = "You escaped";
    [SerializeField]
    private string defeatMessage = "The monster got Clebinho";
    [SerializeField]
    private ObjectiveType objectiveType;
    [SerializeField]
    private List<Mission> baseMissions;

    public string Title => title;
    public string Description => description;
    public string VictoryMessage => victoryMessage;
    public string DefeatMessage => defeatMessage;

    public bool IsCompleted { get; private set; }
    public float Progress { get; private set; }
    public List<Mission> Missions { get; private set; }

    public SelectableType Type => SelectableType.Objective;
    public ObjectiveType ObjectiveType => objectiveType;


    private SignalBus signalBus;
    private int missionsCompleted;

    public void CompleteMission(Mission mission)
    {
        if (Missions.Contains(mission))
        {
            missionsCompleted++;
            if(missionsCompleted == Missions.Count)
            {
                CompleteObjective();
            }
        }
        else
        {
            Debug.LogError("Completed mission was not in the active objective");
        }
    }

    public void CompleteObjective()
    {
        IsCompleted = true;
        signalBus.Fire(new OnGameCompletedSignal(this, true));
    }

    public void Clear()
    {
        Progress = 0;
    }

    public List<ItemType> GetRequiredItems()
    {
        List<ItemType> items = new List<ItemType>();
        foreach(Mission mission in Missions)
        {
            items.AddRange(mission.GetRequiredItems());
        }
        return items;
    }

    public void SetUp(Objective objective)
    {
        description = objective.Description;
        IsCompleted = objective.IsCompleted;
        Progress = objective.Progress;
        baseMissions = objective.baseMissions;
        Missions = new List<Mission>();
    }

    public void StartObjective(SignalBus signalBus)
    {
        Missions.Clear();
        foreach (Mission mission in baseMissions)
        {
            Mission newMission = Mission.CreateInstance<Mission>();
            newMission.SetUp(mission.ItemData, signalBus);
            Missions.Add(newMission);
        }

        this.signalBus = signalBus;
        missionsCompleted = 0;
        IsCompleted = false;
    }

    public void UpdateProgress(float amount)
    {
        Progress += amount;
        if(Progress >= 1)
        {
            CompleteObjective();
        }
    }
}