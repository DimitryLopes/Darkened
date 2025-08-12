using Zenject;
using System;

public class FloatingTextFactoryInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<FloatingTextFactory>().AsSingle();
    }
}
