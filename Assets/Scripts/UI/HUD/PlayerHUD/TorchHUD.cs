using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class TorchHUD : Activateable
{
    [Inject]
    private SignalBus signalBus;
    [Inject]
    private GameManager gameManager;

    [SerializeField]
    private Image torchFillImage;

    private PlayerTorch playerTorch;

    private void Awake()
    {
        signalBus.Subscribe<OnPlayerSpawnedSignal>(OnPlayerSpawned);
    }

    private void OnPlayerSpawned(OnPlayerSpawnedSignal signal)
    {
        playerTorch = signal.Player.Torch;
    }

    private void Update()
    {
        if (!gameManager.IsPlaying) return;

        torchFillImage.fillAmount = playerTorch.CurrentLifeTime / Constants.Entities.TORCH_LIFETIME;
    }
}
