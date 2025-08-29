using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class TorchHUD : Activateable
{
    [Inject]
    private SignalBus signalBus;

    [SerializeField]
    private Image torchFillImage;

    private PlayerTorch playerTorch;

    private void Start()
    {
        signalBus.Subscribe<OnPlayerSpawnedSignal>(OnPlayerSpawned);
    }

    private void OnPlayerSpawned(OnPlayerSpawnedSignal signal)
    {
        playerTorch = signal.Player.Torch;
    }

    private void Update()
    {
        torchFillImage.fillAmount = playerTorch.CurrentLifeTime / playerTorch.MaxLifeTime;
    }
}
