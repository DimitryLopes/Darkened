using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Objective : ScriptableObject, IUISelectable
{
    [SerializeField]
    private string description;
    [SerializeField]
    private string title;
    [SerializeField]
    private ObjectiveType objectiveType;
    [SerializeField]
    private List<Mission> baseMissions;

    public string Title => title;
    public string Description => description;
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
        signalBus.Fire(new OnObjectiveCompletedSignal(this));
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
        this.description = objective.Description;
        this.IsCompleted = objective.IsCompleted;
        this.Progress = objective.Progress;
        this.baseMissions = objective.baseMissions;
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