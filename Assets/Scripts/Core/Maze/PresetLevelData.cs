using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MazeData", menuName = "Scriptable Objects/Datas/Preset Level Data")]
public class PresetLevelData : ScriptableObject
{
    [SerializeField, Header("Size")]
    private MazeSizeData sizeData;

    [SerializeField, Header("Objective")]
    private ObjectiveData objectiveData;

    [SerializeField, Header("Walls")]
    private List<PresetWallPositionData> wallPositions;

    [SerializeField, Header("Items")]
    private List<PresetItemData> items;

    [SerializeField, Header("Core positions")]
    private Coordinate startPosition;
    [SerializeField]
    private Coordinate enemySpawnPosition;

    public MazeSizeData SizeData => sizeData;
    public ObjectiveData ObjectiveData => objectiveData;
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

    [SerializeField, HideInInspector]
    private SpawnType SpawnType;


    public Coordinate Coordinate => coordinate;
    public Item Item => item;
    public Cardinal Direction => direction;
}

[Serializable]
public struct PresetWallPositionData
{
    [SerializeField]
    private Coordinate coordinate;
    [SerializeField]
    private Cardinal direction;

    public Coordinate Coordinate => coordinate;
    public Cardinal Direction => direction;

    public PresetWallPositionData(Coordinate coordinate, Cardinal direction)
    {
        this.coordinate = coordinate;
        this.direction = direction;
    }
}
