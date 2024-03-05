using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class MazeManager 
{
    private readonly MazeGenerator mazeGenerator;
    private readonly ItemFactory itemFactory;

    public void LoadMaze(MazeData data)
    {
        mazeGenerator.CreateMaze(data);
    }

    public Item CreateItem(ItemType type)
    {
        return itemFactory.Create(type);
    }

    public List<MissionItem> GetMissionItems(Objective objective)
    {
        List<ItemType> missionItemsInfo = objective.GetRequiredItems();
        List<MissionItem> items = new List<MissionItem>();
        foreach(ItemType type in missionItemsInfo)
        {
            MissionItem missionItem = (MissionItem)CreateItem(type);
            missionItem.SetObjective(objective);
            items.Add(missionItem);
        }
        return items;
    }

    [Inject]
    public MazeManager(ItemFactory itemFactory, MazeGenerator mazeGenerator)
    {
        this.itemFactory = itemFactory;
        this.mazeGenerator = mazeGenerator;
    }
}
