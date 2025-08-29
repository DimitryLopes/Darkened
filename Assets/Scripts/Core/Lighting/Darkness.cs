using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Darkness : MonoBehaviour
{
    [Inject]
    private CameraManager cameraManager; //will be used later for visual effects
    [Inject]
    private SignalBus signalBus;
    [Inject]
    private GameManager gameManager;

    private float darknessTimer = 0f;
    private float maxDarknessTime;
    private Player player;

    private bool IsPlayerInDarkness => !player.IsLit;
    private bool wasPlayerInDarkness;

    public void Start()
    {
        signalBus.Subscribe<OnMazeLoadFinishSignal>(OnMazeLoadFinish);
        signalBus.Subscribe<OnPlayerSpawnedSignal>(OnPlayerSpawned);
    }

    private void OnPlayerSpawned(OnPlayerSpawnedSignal signal)
    {
        darknessTimer = 0f;
        player = signal.Player;
    }

    private void OnMazeLoadFinish(OnMazeLoadFinishSignal signal)
    {
        maxDarknessTime = signal.Maze.Data.DifficultyData.MaxDarknessTime;
    }

    private void Update() 
    { 
        if (!gameManager.IsPlaying) return;

        if (IsPlayerInDarkness)
        {
            darknessTimer += Time.deltaTime;
            if (darknessTimer >= maxDarknessTime)
            {
                signalBus.Fire(new OnGameCompletedSignal(false));
            }
            if(!wasPlayerInDarkness)
            {
                wasPlayerInDarkness = true;
            }
        }
        else if(wasPlayerInDarkness)
        {
            darknessTimer = 0f;
            wasPlayerInDarkness = false;
        }
    }
}
