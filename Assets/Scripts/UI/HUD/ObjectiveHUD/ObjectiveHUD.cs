using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ObjectiveHUD : Activateable
{
    [Inject]
    private SignalBus signalBus;

    [SerializeField]
    private UIMissionDescription missionDescriptionPrefab;
    [SerializeField]
    private Transform descriptionsContainer;

    private List<UIMissionDescription> instantiatedDescriptions = new List<UIMissionDescription>();

    private void Awake()
    {
        signalBus.Subscribe<OnMissionGroupCompletedSignal>(OnMissionCompleted);
        signalBus.Subscribe<OnMissionGroupStartedSignal>(OnMissionGroupStarted);
        signalBus.Subscribe<OnGameCompletedSignal>(OnGameCompleted);
    }

    private void OnMissionCompleted(OnMissionGroupCompletedSignal signal)
    {
        foreach (UIMissionDescription missionDescription in instantiatedDescriptions)
        {
            if (missionDescription.IsActive)
            {
                missionDescription.Deactivate();
            }
        }
    }

    private void OnMissionGroupStarted(OnMissionGroupStartedSignal signal)
    {
        foreach(Mission mission in signal.MissionGroup.Missions)
        {
            UIMissionDescription description = GetAvailableDescription();
            description.SetUp(mission, signalBus);
        }
    }

    private void OnGameCompleted()
    {
        foreach (UIMissionDescription missionDescription in instantiatedDescriptions)
        {
            if (missionDescription.IsActive)
            {
                missionDescription.Deactivate();
            }
        }
    }

    public UIMissionDescription GetAvailableDescription()
    {
        foreach(UIMissionDescription missionDescription in instantiatedDescriptions)
        {
            if (!missionDescription.IsActive)
            {
                missionDescription.Activate();
                return missionDescription;
            }
        }

        UIMissionDescription newMissionDescription = Instantiate(missionDescriptionPrefab, descriptionsContainer);
        newMissionDescription.Activate();
        instantiatedDescriptions.Add(newMissionDescription);
        return newMissionDescription;
    }
}
