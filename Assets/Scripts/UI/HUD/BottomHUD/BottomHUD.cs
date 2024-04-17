using System;
using System.Collections;
using System.Collections.Generic;
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

    private void Start()
    {
        useItemButton.onClick.AddListener(OnUseButtonClicked);
        nextItemButton.onClick.AddListener(OnNextButtonClicked);
        previousItemButton.onClick.AddListener(OnPreviousButtonClicked);
    }

    public void UpdateButtonHUD(bool hasUse)
    {
        useItemButton.interactable = hasUse;
    }

    public override void OnActivate()
    {
        useItemButton.interactable = false;
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
        signalBus.Fire(new OnBottomHUDUseButtonClickedSignal());
    }
}
