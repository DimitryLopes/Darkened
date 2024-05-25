using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ObjectiveHUD : Activateable
{
    [Inject]
    private SignalBus signalBus;
    [Inject]
    private Coroutiner coroutiner;

    [SerializeField]
    private UIMissionDescription missionDescriptionPrefab;
    [SerializeField]
    private LayoutGroup descriptionsContainer;

    private List<UIMissionDescription> instantiatedDescriptions = new List<UIMissionDescription>();

    private WaitForEndOfFrame waitForFrame = new WaitForEndOfFrame();

    private void Awake()
    {
        signalBus.Subscribe<OnMissionGroupCompletedSignal>(OnMissionCompleted);
        signalBus.Subscribe<OnMissionGroupStartedSignal>(OnMissionGroupStarted);
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
        foreach(IMission mission in signal.MissionGroup.Missions)
        {
            UIMissionDescription description = GetAvailableDescription();
            description.SetUp(mission, signalBus);
        }

        descriptionsContainer.CalculateLayoutInputVertical();
        coroutiner.RunCoroutine(OnHudUpdated());
    }

    private IEnumerator OnHudUpdated()
    {
        yield return waitForFrame;
        Canvas.ForceUpdateCanvases();
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();
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

        UIMissionDescription newMissionDescription = Instantiate(missionDescriptionPrefab, descriptionsContainer.transform);
        newMissionDescription.Activate();
        instantiatedDescriptions.Add(newMissionDescription);
        return newMissionDescription;
    }
}
