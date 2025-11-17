using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Rename Me", menuName = "Scriptable Objects/Settings/Difficulty")]
public class DifficultyData : ScriptableObject, IUISelectable
{
    [SerializeField]
    private DifficultyType difficulty;
    [SerializeField]
    private string difficultyName;
    [SerializeField, Range(0, 2), Header("Torches")]
    private float torchRadius;
    [SerializeField, Range(0, 1), Tooltip("Minimum toch percentage based on map size")]
    private float minTorchRatio;
    [SerializeField, Range(0, 1), Tooltip("Maximum toch percentage based on map size")]
    private float maxTorchRatio;
    [SerializeField, Range(0, 1), Tooltip("Ration of torches that should be lit on start")]
    private float litTorchRatio;

    [SerializeField, Header("Core")]
    private float maxDarknessTime;

    [SerializeField, Header("Player")]
    private float playerTorchBurnSpeedModifier;

    [SerializeField, Header("Items")]
    private float spiritMovementSpeedModifier;

    [SerializeField, Header("Traps")]
    private float darknessDartTorchReductionOnHit;

    [SerializeField]
    private List<ItemGenerationData> difficultyItemDatas;

    public float MinimumTorchRatio => minTorchRatio;
    public float LitTorchRatio => litTorchRatio;
    public float MaximumTorchRatio => maxTorchRatio;
    public float TorchRadius => torchRadius;
    public string Title => difficultyName;
    public List<ItemGenerationData> DifficultyItemDatas => difficultyItemDatas;
    public float PlayerTorchBurnSpeedModifier => playerTorchBurnSpeedModifier;
    public float MaxDarknessTime => maxDarknessTime;
    public DifficultyType DifficultyType => difficulty;
    public SelectableType SelectableType => SelectableType.Difficulty;
    public float SpiritMovementSpeedModifier => spiritMovementSpeedModifier;
    public float DarknessDartTorchReductionOnHit => darknessDartTorchReductionOnHit;
}
