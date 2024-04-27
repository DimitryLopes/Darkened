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
    [SerializeField]
    private LayoutGroup layoutGroup;

    private IMission mission;
    private SignalBus signalBus;

    public void SetUp(IMission mission, SignalBus signalBus)
    {
        this.mission = mission;
        this.signalBus = signalBus;

        signalBus.Subscribe<OnMissionCompletedSignal>(OnMissionCompleted);
        signalBus.Subscribe<OnMissionProgressSignal>(OnMissionProgress);

        UpdateText(mission);
        layoutGroup.CalculateLayoutInputVertical();
    }

    public void UpdateText(IMission mission)
    {
        progressImage.fillAmount = mission.GetCurrentProgress();
        descrptionText.text = mission.Description;
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
