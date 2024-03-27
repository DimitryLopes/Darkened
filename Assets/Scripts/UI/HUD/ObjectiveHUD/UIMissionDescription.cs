using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;

public class UIMissionDescription : Activateable
{
    [SerializeField]
    private TextMeshProUGUI descrptionText;
    [SerializeField]
    private Image progressImage;

    private Mission mission;
    private SignalBus signalBus;

    public void SetUp(Mission mission, SignalBus signalBus)
    {
        this.mission = mission;
        this.signalBus = signalBus;

        signalBus.Subscribe<OnMissionCompletedSignal>(OnMissionCompleted);
        signalBus.Subscribe<OnMissionProgressSignal>(OnMissionProgress);

        UpdateText(mission);
    }

    public void UpdateText(Mission mission)
    {
        progressImage.fillAmount = mission.Progress;
        descrptionText.text = string.Format(Constants.Hud.UI_MISSION_DESCRIPTION_FORMAT, mission.Data.Description, mission.RawProgress, mission.Items.Count);
    }

    public void OnMissionProgress(OnMissionProgressSignal signal)
    {
        if (signal.mission == mission)
        {
            UpdateText(signal.mission);
        }
    }

    public void OnMissionCompleted(OnMissionCompletedSignal signal)
    {
        if (signal.Mission == mission)
        {
            DoHideAnimation();
        }
    }

    public override void OnDeactivate()
    {
        signalBus.Unsubscribe<OnMissionProgressSignal>(OnMissionProgress);
        signalBus.Unsubscribe<OnMissionCompletedSignal>(OnMissionCompleted);
    }

    public void DoHideAnimation()
    {
        Deactivate();
    }
}
