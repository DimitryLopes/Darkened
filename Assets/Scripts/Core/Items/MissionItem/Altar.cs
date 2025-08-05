using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using Zenject;

public class Altar : MissionItem
{
    [Inject]
    private InventoryManager inventoryManager;
    [Inject]
    private ObjectiveManager objectiveManager;

    [SerializeField]
    private Light2D altarLight;
    [SerializeField]
    private Image altarFillImage;

    protected override void OnInteract()
    {
        if (!inventoryManager.HasEnoughItem(ItemType.Orb))
        {
            //TODO: SHOW FLOATING TEXT
            return;
        }

        altarLight.enabled = true;
        inventoryManager.DecreaseItemAmount(ItemType.Orb);
        var missions = objectiveManager.CurrentObjective.CurrentMissionGroup.Missions;
        foreach(IMission mission in missions)
        {
            if(mission is ItemMission itemMission)
            {
                if(itemMission.Data.Item == ItemType.Altar)
                {
                    float progress = itemMission.GetCurrentProgress();
                    altarFillImage.fillAmount = progress;
                    altarLight.pointLightOuterRadius = progress * 2f;
                }
            }
        }        
        signalBus.Fire(new OnMissionItemInteractedSignal(this));
    }

    public override void OnActivate()
    {
        base.OnActivate();
        altarLight.pointLightOuterRadius = 0;
        altarLight.enabled = false;
        altarFillImage.fillAmount = 0;
    }

    public override ItemType Type => ItemType.Altar;
}
