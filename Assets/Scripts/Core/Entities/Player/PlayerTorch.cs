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

    private float currentLifeTime;
    private float burnSpeedModifier;

    public float CurrentLifeTime => currentLifeTime;

    public void Setup(SignalBus signalBus)
    {
        this.signalBus = signalBus;

        signalBus.Subscribe<OnTorchAbsorbedSignal>(OnTorchAbsorbed);
    }

    public void SetBurnSpeedModifier(DifficultyData data)
    {
        burnSpeedModifier = data.PlayerTorchBurnSpeedModifier;
    }

    private void Update()
    {
        currentLifeTime -= Time.deltaTime * burnSpeedModifier;
        if (currentLifeTime <= 0)
        {
            currentLifeTime = 0;
            DeactivateTorch();
        }
    }

    private void OnTorchAbsorbed(OnTorchAbsorbedSignal signal)
    {
        if (currentLifeTime <= 0)
        {
            ActivateTorch();
        }
        else
        {
            currentLifeTime = Constants.Player.TORCH_LIFETIME;
        }
    }

    public void ActivateTorch()
    {
        playerTorch.enabled = true;
        currentLifeTime = Constants.Player.TORCH_LIFETIME;
        signalBus.Fire(new OnTorchLitSignal(playerTorch));
    }

    public void DeactivateTorch()
    {
        playerTorch.enabled = false;
        signalBus.Fire(new OnTorchExtinguishedSignal(playerTorch));
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
