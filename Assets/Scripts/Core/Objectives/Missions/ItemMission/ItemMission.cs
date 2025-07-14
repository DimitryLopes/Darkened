using Zenject;
using System.Collections.Generic;

public class ItemMission : Mission<ItemMissionData>
{
    public ItemMission(SignalBus signalBus) : base(signalBus) 
    {
        signalBus.Subscribe<OnMissionItemInteractedSignal>(OnMissionProgress);
    }

    public List<MissionItem> Items { get; set; } = new List<MissionItem>();

    public void OnMissionProgress(OnMissionItemInteractedSignal Signal)
    {
        if (!IsActive) return;

        if (!Items.Contains(Signal.MissionItem)) return;

        progress++;
        UpdateProgress();
    }

    public void SetupMissionData()
    {
        Data.Clear();
        Data.SetUp();
    }

    public List<ItemType> GetRequiredItems()
    {
        List<ItemType> items = new List<ItemType>();

        for (int i = 0; i < Data.ItemDatas[Difficulty].Amount; i++)
        {
            items.Add(Data.Item);
        }
        return items;
    }

    protected override void OnMissionStarted()
    {
        base.OnMissionStarted();
        Items.Clear();
    }

    protected override void OnMissionCompleted()
    {
        base.OnMissionCompleted();
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
