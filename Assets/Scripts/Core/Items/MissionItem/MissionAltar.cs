using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class MissionAltar : MissionItem
{
    [Inject]
    private InventoryManager inventoryManager;
    [Inject]
    private ObjectiveManager objectiveManager;

    [SerializeField]
    private Image altarFillImage;


    protected override void OnInteract()
    {
        if (!inventoryManager.HasEnoughItem(ItemType.Orb))
        {
            //TODO: SHOW FLOATING TEXT
            return;
        }

        signalBus.Fire(new OnMissionItemInteractedSignal(this));
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
                }
            }
        }
        
    }

    public override ItemType Type => ItemType.Altar;
}
