using Zenject;
using System.Collections.Generic;

public class ItemMission : Mission<ItemMissionData>
{
    public ItemMission(SignalBus signalBus) : base(signalBus) 
    {
        signalBus.Subscribe<OnMissionItemInteractedSignal>(OnMissionProgress);
    }

    public override float Progress => (float)progress / (float)Data.ItemData.Amount;

    public List<MissionItem> Items { get; set; } = new List<MissionItem>();

    public void OnMissionProgress(OnMissionItemInteractedSignal Signal)
    {
        if (!IsActive) return;

        if (!Items.Contains(Signal.MissionItem)) return;

        progress++;
        UpdateProgress();
    }

    public List<ItemType> GetRequiredItems()
    {
        List<ItemType> items = new List<ItemType>();

        for (int i = 0; i < Data.ItemData.Amount; i++)
        {
            items.Add(Data.ItemData.ItemType);
        }
        return items;
    }

    protected override void OnMissionStarted()
    {
        base.OnMissionStarted();
        Items.Clear();
    }

    protected override void OnActivate()
    {
        foreach (MissionItem item in Items)
        {
            item.EnableInteraction();
        }
    }

    protected override void OnDeactivate()
    {
        foreach (MissionItem item in Items)
        {
            item.DisableInteraction();
        }
    }
}
