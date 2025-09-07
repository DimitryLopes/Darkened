using Zenject;

public class EntityFactoryInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<PlayerFactory>().AsSingle();
        Container.Bind<VisualStatusEffectFactory>().AsSingle();
    }
}
