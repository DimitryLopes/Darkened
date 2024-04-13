using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Objective", menuName = "Scriptable Objects/Objective")]
public class ObjectiveData : ScriptableObject, IUISelectable
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
    private List<MissionGroupData> missionGroups;

    public string Title => title;
    public string Description => description;
    public string VictoryMessage => victoryMessage;
    public string DefeatMessage => defeatMessage;
    public List<MissionGroupData> MissionGroups => missionGroups;
    public ObjectiveType ObjectiveType => objectiveType;
    public SelectableType SelectableType => SelectableType.Objective;
}