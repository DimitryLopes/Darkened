using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class MazeManager
{
    private readonly MazeGenerator mazeGenerator;
    private readonly ItemFactory itemFactory;
    private readonly ObjectiveManager objectiveManager;

    public MazeNode CurrentStartingNode { get; set; }
    public MazeNode EnemyStartingNode { get; set; }
    public Maze CurrentMaze { get; set; }

    public Dictionary<ItemType, List<Item>> ItemDictionary = new Dictionary<ItemType, List<Item>>();

    [Inject]
    public MazeManager(ObjectiveManager objectiveManager, ItemFactory itemFactory, MazeGenerator mazeGenerator, SignalBus signalBus)
    {
        this.objectiveManager = objectiveManager;
        this.mazeGenerator = mazeGenerator;
        this.itemFactory = itemFactory;

        signalBus.Subscribe<OnMazeLoadStartedSignal>(OnMazeLoadStarted);
    }

    public void LoadMaze(LevelData data)
    {
        ClearItems();
        objectiveManager.StartObjective(data.ObjectiveData);
        Debug.Log($"Generating Maze with data following data: \n Size: {data.Size} cells \n Min torches: {data.MinTorchCount} \n Max torches: {data.MaxTorchCount}");
        mazeGenerator.CreateMaze(data);
    }

    private void OnMazeLoadStarted(OnMazeLoadStartedSignal signal)
    {
        CurrentMaze = signal.Maze;
    }

    private void ClearItems()
    {
        List<ItemType> types = EnumUtils.GetEnumValues<ItemType>();
        foreach(ItemType type in types)
        {
            if (ItemDictionary.ContainsKey(type))
            {
                foreach(Item item in ItemDictionary[type])
                {
                    item.Deactivate();
                }
            }
        }
    }

    public Item GetAvailableItem(ItemType type)
    {
        if (!ItemDictionary.ContainsKey(type))
        {
            ItemDictionary.Add(type, new List<Item>());
        }

        foreach(Item item in ItemDictionary[type])
        {
            if (!item.IsActive)
            {
                item.Activate();
                return item;
            }
        }

        Item newItem = itemFactory.Create(type);
        ItemDictionary[type].Add(newItem);
        newItem.SetType(type);
        newItem.Activate();
        return newItem;
    }

    public List<MissionItem> GetMissionItems()
    {
        List<MissionItem> items = new List<MissionItem>();
        foreach (MissionGroup missionGroup in objectiveManager.CurrentObjective.MissionGroups)
        {
            foreach (IMission baseMission in missionGroup.Missions)
            {
                if (baseMission is ItemMission mission)
                {
                    List<ItemType> missionItemsInfo = mission.GetRequiredItems();
                    foreach (ItemType type in missionItemsInfo)
                    {
                        MissionItem missionItem = (MissionItem)GetAvailableItem(type);
                        items.Add(missionItem);
                        objectiveManager.AddMissionToItem(missionItem, mission);
                    }
                }
            }
        }
        return items;
    }

    public MazeTorch GetMazeTorch(LevelData data)
    {
        MazeTorch torch = (MazeTorch)GetAvailableItem(ItemType.DefaultTorch);
        torch.SetLightRadius(data.DifficultyData.TorchRadius);
        return torch;
    }

}
