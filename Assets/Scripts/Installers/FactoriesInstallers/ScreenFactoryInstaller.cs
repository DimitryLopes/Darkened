using Zenject;

public class ScreenFactoryInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<ScreenFactory>().AsSingle();
    }
}
