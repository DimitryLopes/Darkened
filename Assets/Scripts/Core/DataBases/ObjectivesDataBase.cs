using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObjectivesDataBase", menuName = "Scriptable Objects/Data Bases/Objective Data Base")]
public class ObjectivesDataBase : ScriptableObject
{
    [SerializeField]
    private List<Objective> objectives;

    public Dictionary<ObjectiveType, Objective> ObjectivesDictionary = new Dictionary<ObjectiveType, Objective>();
    public List<Objective> Objectives => objectives;

    public void SetUp()
    {
        foreach(Objective objective in objectives)
        {
            ObjectivesDictionary.Add(objective.ObjectiveType, objective);
        }
    }

    public Objective GetRandomObjective()
    {
        return objectives.GetRandom();
    }

    public List<Objective> GetAllObjectives()
    {
        return objectives;
    }

    public Objective GetObjective(ObjectiveType type)
    {
        return ObjectivesDictionary[type];
    }
}

public enum ObjectiveType
{
    FindExit,
    PressButtons,
    KillMonster,
    Survive,
}