using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class BottomHUD : Activateable
{
    [Inject]
    private SignalBus signalBus;

    [SerializeField]
    private Button useItemButton;
    [SerializeField]
    private Button nextItemButton;
    [SerializeField]
    private Button previousItemButton;
    [SerializeField]
    private UIInteractionButton interactButton;
    [SerializeField]
    private UIToggleButton sprintButton;

    public UIToggleButton SprintButton => sprintButton;

    private void Start()
    {
        useItemButton.onClick.AddListener(OnUseButtonClicked);
        nextItemButton.onClick.AddListener(OnNextButtonClicked);
        previousItemButton.onClick.AddListener(OnPreviousButtonClicked);
        interactButton.Button.onClick.AddListener(OnInteractionButtonClicked);
    }

    private void OnEnable()
    {
        signalBus.Subscribe<OnPlayerInteractableChangedSignal>(OnInteractableChanged);
    }

    private void OnDisable()
    {
        signalBus.TryUnsubscribe<OnPlayerInteractableChangedSignal>(OnInteractableChanged);
        if (!sprintButton.IsToggled) return;
        sprintButton.Toggle();
    }

    public void UpdateButtonHUD(bool hasUse)
    {
        useItemButton.interactable = hasUse;
    }

    public override void Activate(bool forced = false)
    {
        if (!GameManager.IsOnPhone) return;
        base.Activate(forced);
    }

    public override void OnActivate()
    {
        base.OnActivate();
        useItemButton.interactable = false;
    }

    private void OnInteractableChanged(OnPlayerInteractableChangedSignal signal)
    {
        if(signal.Item != null)
        {
            interactButton.SetIcon(signal.Item.Icon);
            return;
        }

        interactButton.SetIcon(null);
    }

    private void OnInteractionButtonClicked()
    {
        signalBus.Fire(new OnInteractionButtonClickedSignal());
    }

    private void OnNextButtonClicked()
    {
        signalBus.Fire(new OnBottomHUDNextButtonClickedSignal());
    }

    private void OnPreviousButtonClicked()
    {
        signalBus.Fire(new OnBottomHUDPreviousButtonClickedSignal());
    }

    private void OnUseButtonClicked()
    {
        signalBus.Fire(new OnPlayerItemUsedSignal());
    }
}
