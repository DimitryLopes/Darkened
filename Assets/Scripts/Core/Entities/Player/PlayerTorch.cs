using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Zenject;

public class PlayerTorch : MonoBehaviour
{
    private SignalBus signalBus;

    [SerializeField]
    private Light2D playerTorch;
    [SerializeField]
    private Color defaultTorchColor;
    [SerializeField]
    private Color spectalTorchColor;

    private float maxLifeTime;
    private float currentLifeTime;
    private float burnSpeedModifier;

    public void Setup(SignalBus signalBus)
    {
        this.signalBus = signalBus;

        signalBus.Subscribe<OnMazeLoadFinishSignal>(OnMazeLoadFinish);
        signalBus.Subscribe<OnTorchAbsorbedSignal>(OnTorchAbsorbed);
    }

    private void Update()
    {
        currentLifeTime -= Time.deltaTime * burnSpeedModifier;
        if (currentLifeTime <= 0)
        {
            currentLifeTime = 0;
            DeactivateTorch();
            signalBus.Fire(new OnPlayerTorchExtinguishedSignal());
        }
    }

    private void OnTorchAbsorbed(OnTorchAbsorbedSignal signal)
    {
        currentLifeTime = maxLifeTime;
        signalBus.Fire(new OnPlayerTorchLitSignal());
    }

    private void OnMazeLoadFinish(OnMazeLoadFinishSignal signal)
    {
        maxLifeTime = signal.Maze.Data.DifficultyData.PlayerTorchLifeTime;
        burnSpeedModifier = signal.Maze.Data.DifficultyData.PlayerTorchBurnSpeedModifier;
        currentLifeTime = maxLifeTime;
    }

    public void ActiveTorch()
    {
        playerTorch.enabled = true;
    }

    public void DeactivateTorch()
    {
        playerTorch.enabled = false;
    }

    public void SetBigTorch()
    {
        playerTorch.pointLightOuterRadius = Constants.Player.PLAYER_BIG_TORCH_SIZE;
    }

    public void SetDefaultTorch()
    {
        playerTorch.pointLightOuterRadius = Constants.Player.PLAYER_DEFAULT_TORCH_SIZE;
        playerTorch.shadowsEnabled = true;
        playerTorch.color = defaultTorchColor;
    }

    public void SetSpectralTorch()
    {
        playerTorch.shadowsEnabled = false;
        playerTorch.color = spectalTorchColor;
    }
}
