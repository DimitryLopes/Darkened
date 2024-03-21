using Zenject;

public class UIFactoryInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<UIFactory>().AsSingle();
    }
}
