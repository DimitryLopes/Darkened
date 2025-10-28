using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class MazeManager
{
    private readonly MazeGenerator mazeGenerator;
    private readonly ItemFactory itemFactory;
    private readonly ObjectiveManager objectiveManager;
    private readonly SignalBus signalbus;

    public Node CurrentStartingNode { get; set; }
    public Node EnemyStartingNode { get; set; }
    public Maze CurrentMaze { get; set; }
    public MazeGenerator MazeGenerator => mazeGenerator;

    public Dictionary<ItemType, List<Item>> ItemDictionary = new Dictionary<ItemType, List<Item>>();

    [Inject]
    public MazeManager(ObjectiveManager objectiveManager, ItemFactory itemFactory, MazeGenerator mazeGenerator, SignalBus signalBus)
    {
        this.objectiveManager = objectiveManager;
        this.mazeGenerator = mazeGenerator;
        this.itemFactory = itemFactory;
        this.signalbus = signalBus;

        signalBus.Subscribe<OnMazeLoadStartedSignal>(OnMazeLoadStarted);
    }

    public void LoadMaze(LevelData data)
    {
        ClearItems();
        objectiveManager.StartObjective(data.ObjectiveData, data.DifficultyData.DifficultyType);
        if(data is RandomLevelData randomData)
        {
            Debug.Log($"Generating Maze with data following data: \n Size: {randomData.Size} cells \n Min torches: {randomData.MinTorchCount} \n Max torches: {randomData.MaxTorchCount}");
            mazeGenerator.CreateMaze(data as RandomLevelData, this);
        }
        else
        {
            mazeGenerator.CreateMaze(data as PresetLevelData, this);
            Debug.Log($"Generating Maze with data following data: \n Size: {data.Size} cells");
        }
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

    public List<Item> GetMissionItems()
    {
        List<Item> items = new List<Item>();
        foreach (MissionGroup missionGroup in objectiveManager.CurrentObjective.MissionGroups)
        {
            foreach (IMission baseMission in missionGroup.Missions)
            {
                if (baseMission is ItemMission mission)
                {
                    mission.SetupMissionData();
                    List<ItemType> missionItemsInfo = mission.GetRequiredItems();
                    foreach (ItemType type in missionItemsInfo)
                    {
                        MissionItem missionItem = (MissionItem)GetAvailableItem(type);
                        items.Add(missionItem);
                        objectiveManager.AddItemToMission(missionItem, mission);
                    }
                }
            }
        }
        return items;
    }

    public List<ItemPositionData> GetPresetMissionItems(PresetLevelData preset)
    {
        List<ItemPositionData> items = new List<ItemPositionData>();

        // Obtém todas as missões de item do objetivo atual
        var itemMissions = new List<ItemMission>();
        foreach (MissionGroup missionGroup in objectiveManager.CurrentObjective.MissionGroups)
        {
            foreach (IMission baseMission in missionGroup.Missions)
            {
                if (baseMission is ItemMission mission)
                {
                    mission.SetupMissionData();
                    itemMissions.Add(mission);
                }
            }
        }

        // Para cada item do preset, verifica se pertence a alguma missão
        foreach (var presetItemData in preset.Items)
        {
            foreach (var mission in itemMissions)
            {
                if (presetItemData.Item.Type == mission.Data.Item)
                {
                    // Instancia o item e associa à missão
                    MissionItem missionItem = (MissionItem)GetAvailableItem(presetItemData.Item.Type);
                    items.Add(new ItemPositionData(missionItem, presetItemData));
                    objectiveManager.AddItemToMission(missionItem, mission);
                }
            }
        }
        return items;
    }

    public void SetupMissions()
    {
        foreach (MissionGroup missionGroup in objectiveManager.CurrentObjective.MissionGroups)
        {
            foreach (IMission baseMission in missionGroup.Missions)
            {
                switch (baseMission)
                {
                    case ItemMission itemMission:
                        itemMission.SetupMissionData();
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public Torch GetMazeTorch(LevelData data)
    {
        Torch torch = (Torch)GetAvailableItem(ItemType.DefaultTorch);
        torch.SetLightRadius(data.DifficultyData.TorchRadius);
        return torch;
    }

}
