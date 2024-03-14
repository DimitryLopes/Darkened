using Zenject;
using System;

public class ItemFactoryInstaller : MonoInstaller
{
    [Inject]
    private ItemDataBase itemDataBase;

    public override void InstallBindings()
    {
        Container.Bind<ItemFactory>().AsSingle();
        Container.Bind<Item>().FromInstance(itemDataBase.ItemDatas[ItemType.Exit]).WhenInjectedInto<ItemFactory>();
        Container.Bind<Item>().FromInstance(itemDataBase.ItemDatas[ItemType.DefaultTorch]).WhenInjectedInto<ItemFactory>();
    }
}