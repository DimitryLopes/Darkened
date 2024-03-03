using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class MazeManager 
{
    private readonly MazeGenerator mazeGenerator;
    private readonly Item.Factory itemFactory;

    public void LoadMaze(MazeData data)
    {
        mazeGenerator.CreateMaze(data);
    }

    public void CreateItem(ItemType type)
    {
        //MazeExit item = itemFactory.Create();
    }

    [Inject]
    public MazeManager(Item.Factory itemFactory, MazeGenerator mazeGenerator)
    {
        this.itemFactory = itemFactory;
        this.mazeGenerator = mazeGenerator;
    }
}
