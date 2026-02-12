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
    [SerializeField]
    private float defaultSize = 0.16f;
    [SerializeField]
    private float bigSize = 0.24f;

    private float currentLifeTime;
    private float burnSpeedModifier;

    public float CurrentLifeTime => currentLifeTime;
    public bool CanBurn { get; set; }

    public void Setup(SignalBus signalBus)
    {
        this.signalBus = signalBus;

        signalBus.Subscribe<OnTorchAbsorbedSignal>(OnTorchAbsorbed);
    }

    public void SetBurnSpeedModifier(DifficultyData data)
    {
        burnSpeedModifier = data.PlayerTorchBurnSpeedModifier;
    }

    public void ChangeCurrentLifeTime(float value)
    {
        bool isPositive = value > 0;
        
        if(isPositive && currentLifeTime == 0)
        {
            ActivateTorch();
        
        }
        currentLifeTime += value;

        if (currentLifeTime > Constants.Entities.TORCH_LIFETIME)
        {
            currentLifeTime = Constants.Entities.TORCH_LIFETIME;
        }
        else if (currentLifeTime < 0)
        {
            currentLifeTime = 0;
            DeactivateTorch();
        }
    }

    private void Update()
    {
        if (!CanBurn) return;

       // currentLifeTime -= Time.deltaTime * burnSpeedModifier;
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
            currentLifeTime = Constants.Entities.TORCH_LIFETIME;
        }
    }

    public void ActivateTorch()
    {
        playerTorch.enabled = true;
        currentLifeTime = Constants.Entities.TORCH_LIFETIME;
        signalBus.Fire(new OnTorchLitSignal(playerTorch));
    }

    public void DeactivateTorch()
    {
        playerTorch.enabled = false;
        signalBus.Fire(new OnTorchExtinguishedSignal(playerTorch));
    }

    public void SetBigTorch()
    {
        playerTorch.pointLightOuterRadius = bigSize;
    }

    public void SetDefaultTorch()
    {
        playerTorch.pointLightOuterRadius = defaultSize;
        playerTorch.shadowsEnabled = true;
        playerTorch.color = defaultTorchColor;
    }

    public void SetSpectralTorch()
    {
        playerTorch.shadowsEnabled = false;
        playerTorch.color = spectalTorchColor;
    }
}
