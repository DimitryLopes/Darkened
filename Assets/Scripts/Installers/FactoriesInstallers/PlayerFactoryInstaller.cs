using Zenject;

public class PlayerFactoryInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<PlayerFactory>().AsSingle();
    }
}
