using System.Collections.Generic;
using Zenject;

public class MazeManager
{
    private readonly MazeGenerator mazeGenerator;
    private readonly ItemFactory itemFactory;
    private readonly ObjectiveManager objectiveManager;
    private readonly SignalBus signalBus;

    public MazeNode CurrentStartingNode { get; set; }

    public Dictionary<ItemType, List<Item>> ItemDictionary = new Dictionary<ItemType, List<Item>>();

    public void LoadMaze(MazeData data)
    {
        ClearItems();
        objectiveManager.SetObjective(data.Objective);
        mazeGenerator.CreateMaze(data);
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

    public Item CreateItem(ItemType type)
    {
        if (!ItemDictionary.ContainsKey(type))
        {
            ItemDictionary.Add(type, new List<Item>());
        }

        foreach(Item item in ItemDictionary[type])
        {
            if (!item.IsActive)
            {
                return item;
            }
        }

        Item newItem = itemFactory.Create(type);
        ItemDictionary[type].Add(newItem);
        newItem.SetType(type);
        return newItem;
    }

    public List<MissionItem> GetMissionItems(Objective objective)
    {
        List<MissionItem> items = new List<MissionItem>();
        foreach (Mission mission in objective.Missions)
        {
            List<ItemType> missionItemsInfo = mission.GetRequiredItems();
            foreach (ItemType type in missionItemsInfo)
            {
                MissionItem missionItem = (MissionItem)CreateItem(type);
                items.Add(missionItem);
                objectiveManager.AddMissionToItem(missionItem, mission);
            }
        }
        return items;
    }

    [Inject]
    public MazeManager(ObjectiveManager objectiveManager, ItemFactory itemFactory, MazeGenerator mazeGenerator, SignalBus signalBus)
    {
        this.objectiveManager = objectiveManager;
        this.mazeGenerator = mazeGenerator;
        this.itemFactory = itemFactory;
        this.signalBus = signalBus;
    }
}
