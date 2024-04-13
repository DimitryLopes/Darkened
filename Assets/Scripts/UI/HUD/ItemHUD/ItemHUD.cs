using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ItemHUD : Activateable
{
    [Inject]
    private SignalBus signalBus;

    [SerializeField]
    private Transform itemViewContainer;

    public Transform ItemViewContainer => itemViewContainer;

    private void Start()
    {

        signalBus.Subscribe<OnInventoryItemSelectedSignal>(OnItemSelected);
    }

    private void OnItemSelected()
    {

    }
}
