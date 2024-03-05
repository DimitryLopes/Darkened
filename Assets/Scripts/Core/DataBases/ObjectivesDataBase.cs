using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObjectivesDataBase", menuName = "Scriptable Objects/Data Bases/Objective Data Base")]
public class ObjectivesDataBase : ScriptableObject
{
    [SerializeField]
    private List<Objective> objectives;

    public List<Objective> Objectives => objectives;

    public Objective GetRandomObjective()
    {
        return objectives.GetRandom();
    }
}
