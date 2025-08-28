using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Darkness : MonoBehaviour
{
    [Inject]
    private CameraManager cameraManager;
    [Inject]
    private SignalBus signalBus;

    private bool isPlayerTorchExtinguished = false;
    private float darknessTimer = 0f;
    private float maxDarknessTime;
    private Player player;

    private bool IsPlayerInDarkness => isPlayerTorchExtinguished && !player.IsNearTorch;
    public void Start()
    {
        signalBus.Subscribe<OnMazeLoadFinishSignal>(OnMazeLoadFinishSignal);
        signalBus.Subscribe<OnPlayerTorchExtinguishedSignal>(OnPlayerTorchExtinguished);
        signalBus.Subscribe<OnPlayerTorchLitSignal>(OnPlayerTorchLit);
        signalBus.Subscribe<OnPlayerNearTorchSignal>(OnPlayerNearTorch);
        signalBus.Subscribe<OnPlayerSpawnedSignal>(OnPlayerSpawnedSignal);
    }

    private void OnPlayerSpawnedSignal(OnPlayerSpawnedSignal signal)
    {
        isPlayerTorchExtinguished = true;
        darknessTimer = 0f;
        player = signal.Player;
    }

    private void OnMazeLoadFinishSignal(OnMazeLoadFinishSignal signal)
    {
        maxDarknessTime = signal.Maze.Data.DifficultyData.MaxDarknessTime;
    }

    private void OnPlayerTorchExtinguished(OnPlayerTorchExtinguishedSignal signal)
    {
        isPlayerTorchExtinguished = true;
    }

    private void OnPlayerTorchLit(OnPlayerTorchLitSignal signal)
    {
        isPlayerTorchExtinguished = false;
        darknessTimer = 0f;
    }

    private void OnPlayerNearTorch(OnPlayerNearTorchSignal signal)
    {
        darknessTimer = 0f;
    }

    private void Update()
    {
        if (IsPlayerInDarkness)
        {
            darknessTimer += Time.deltaTime;
            if (darknessTimer >= maxDarknessTime)
            {
                signalBus.Fire(new OnGameCompletedSignal(false));
            }
        }
    }
}
