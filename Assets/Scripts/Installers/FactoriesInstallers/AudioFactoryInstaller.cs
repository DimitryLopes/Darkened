using Zenject;
using System;

public class AudioFactoryInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<AudioFactory>().AsSingle();
    }
}
