using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MazeData", menuName = "Scriptable Objects/Datas/Preset Level Data")]
public class PresetLevelData : LevelData
{
    [SerializeField, Header("Walls")]
    private List<PresetWallPositionData> wallPositions;

    [SerializeField, Header("Items")]
    private List<PresetItemData> items;

    [SerializeField, Header("Core positions")]
    private Coordinate startPosition;
    [SerializeField]
    private Coordinate enemySpawnPosition;

    public List<PresetWallPositionData> WallPositions => wallPositions;
    public List<PresetItemData> Items => items;
    public Coordinate StartPosition => startPosition;
    public Coordinate EnemySpawnPosition => enemySpawnPosition;
}

[Serializable]
public struct PresetItemData
{
    [SerializeField]
    private Coordinate coordinate;
    [SerializeField]
    private Item item;
    [SerializeField, ShowIf(nameof(SpawnType), SpawnType.Wall, SpawnType.EdgeWalls)]
    private Cardinal direction;
    [SerializeField, ShowIf(nameof(ItemType), ItemType.Switch)]
    public string AssociateWith;

    [SerializeField, HideInInspector]
    private SpawnType SpawnType;
    [SerializeField, HideInInspector]
    private ItemType ItemType;


    public Coordinate Coordinate => coordinate;
    public Item Item => item;
    public Cardinal Direction => direction;

    public PresetItemData(Coordinate coordinate, Cardinal direction, SpawnType spawnType, ItemType itemType, Item item = null, string associateWith = null)
    {
        this.coordinate = coordinate;
        this.direction = direction;
        this.SpawnType = spawnType;
        this.ItemType = itemType;
        this.item = item;
        this.AssociateWith = associateWith;
    }
}

[Serializable]
public struct PresetWallPositionData
{
    [SerializeField]
    private Coordinate coordinate;
    [SerializeField]
    private Cardinal direction;
    [SerializeField]
    private WallType wallType;

    public Coordinate Coordinate => coordinate;
    public Cardinal Direction => direction;
    public WallType WallType => wallType;

    public PresetWallPositionData(Coordinate coordinate, Cardinal direction, WallType wallType)
    {
        this.coordinate = coordinate;
        this.direction = direction;
        this.wallType = wallType;
    }
}
