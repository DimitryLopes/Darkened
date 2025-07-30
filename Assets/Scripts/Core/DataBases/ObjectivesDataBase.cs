using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObjectivesDataBase", menuName = "Scriptable Objects/Data Bases/Objective Data Base")]
public class ObjectivesDataBase : ScriptableObject
{
    [SerializeField]
    private List<ObjectiveData> objectives;

    public Dictionary<ObjectiveType, ObjectiveData> ObjectivesDictionary = new Dictionary<ObjectiveType, ObjectiveData>();
    public List<ObjectiveData> Objectives => objectives;

    public void SetUp()
    {
        foreach(ObjectiveData objective in objectives)
        {
            ObjectivesDictionary.Add(objective.ObjectiveType, objective);
        }
    }

    public ObjectiveData GetRandomObjective()
    {
        return objectives.GetRandom();
    }

    public List<ObjectiveData> GetAllObjectives()
    {
        return objectives;
    }

    public ObjectiveData GetObjective(ObjectiveType type)
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
    FinishAltar,
}