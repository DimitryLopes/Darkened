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

    public void CreateItem(ItemType type)
    {
        //MazeExit item = itemFactory.Create();
    }

    [Inject]
    public MazeManager(ItemFactory itemFactory, MazeGenerator mazeGenerator)
    {
        this.itemFactory = itemFactory;
        this.mazeGenerator = mazeGenerator;

        MazeExit exit = (MazeExit)itemFactory.Create<MazeExit>(ItemType.Exit);
    }
}
