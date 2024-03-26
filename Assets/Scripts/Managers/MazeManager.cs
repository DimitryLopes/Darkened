using System.Collections.Generic;
using Zenject;

public class MazeManager
{
    private readonly MazeGenerator mazeGenerator;
    private readonly ItemFactory itemFactory;
    private readonly ObjectiveManager objectiveManager;

    public MazeNode CurrentStartingNode { get; set; }

    public Dictionary<ItemType, List<Item>> ItemDictionary = new Dictionary<ItemType, List<Item>>();

    public void LoadMaze(MazeData data)
    {
        ClearItems();
        objectiveManager.StartObjective(data.ObjectiveData);
        mazeGenerator.CreateMaze(data, this);
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
            foreach (Mission mission in missionGroup.Missions)
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
        return items;
    }

    public MazeTorch GetMazeTorch(MazeData data)
    {
        return (MazeTorch)GetAvailableItem(data.Torch);
    }

    [Inject]
    public MazeManager(ObjectiveManager objectiveManager, ItemFactory itemFactory, MazeGenerator mazeGenerator)
    {
        this.objectiveManager = objectiveManager;
        this.mazeGenerator = mazeGenerator;
        this.itemFactory = itemFactory;
    }
}
