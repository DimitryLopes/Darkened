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

    private IMission mission;
    private SignalBus signalBus;

    public void SetUp(IMission mission, SignalBus signalBus)
    {
        this.mission = mission;
        this.signalBus = signalBus;

        signalBus.Subscribe<OnMissionCompletedSignal>(OnMissionCompleted);
        signalBus.Subscribe<OnMissionProgressSignal>(OnMissionProgress);

        UpdateText(mission);
    }

    public void UpdateText(IMission mission)
    {
        progressImage.fillAmount = mission.Progress;
        descrptionText.text = string.Format(Constants.Hud.UI_MISSION_DESCRIPTION_FORMAT, mission.Description, mission.RawProgress, mission.ProgressTarget);
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
