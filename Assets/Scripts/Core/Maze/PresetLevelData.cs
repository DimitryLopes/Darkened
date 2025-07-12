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
    private Coordinate endPosition;
    [SerializeField]
    private Coordinate enemySpawnPosition;
}

[Serializable]
public struct PresetItemData
{
    [SerializeField]
    private Coordinate coordinate;
    [SerializeField]
    private ItemType itemType;
    public Coordinate Coordinate => coordinate;
    public ItemType ItemType => itemType;
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
}
