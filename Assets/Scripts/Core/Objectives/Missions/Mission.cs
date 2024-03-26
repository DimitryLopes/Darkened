using System.Collections.Generic;
using Zenject;

public class Mission
{
    private MissionData data;
    private SignalBus signalBus;
    private int progress;

    public MissionData Data => data;
    public float Progress => (float)progress / (float)Data.ItemData.Amount;
    public int RawProgress => progress;
    public bool IsCompleted { get; private set; }
    public bool IsActive { get; set; }

    public List<MissionItem> Items { get; set; } = new List<MissionItem>();

    public void OnMissionProgress(OnMissionItemInteractedSignal signal)
    {
        if (IsActive)
        {
            if (Items.Contains(signal.MissionItem))
            {
                progress += 1;
                signalBus.Fire(new OnMissionProgressSignal(this));
                if (progress == Data.ItemData.Amount)
                {
                    CompleteMission();
                }
            }
        }
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

    private void CompleteMission()
    {
        IsCompleted = true;
        signalBus.Fire(new OnMissionCompletedSignal(this));
    }

    public void SetActive(bool value)
    {
        IsActive = value;
        foreach(MissionItem item in Items)
        {
            if (value)
            {
                item.EnableInteraction();
            }
            else
            {
                item.DisableInteraction();
            }
        }
    }

    public Mission(MissionData data, SignalBus signalBus)
    {
        this.data = data;
        this.signalBus = signalBus;
        progress = 0;
        Items.Clear();

        signalBus.Subscribe<OnMissionItemInteractedSignal>(OnMissionProgress);
    }
}
