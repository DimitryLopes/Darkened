using Zenject;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemFactoryInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.BindFactory<Item, Item.Factory>().AsSingle();
    }
}