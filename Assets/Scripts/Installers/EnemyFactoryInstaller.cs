using Zenject;

public class EnemyFactoryInstaller : MonoInstaller
{
    [Inject]
    private EnemyDataBase enemyDataBase;
    public override void InstallBindings()
    {
        Container.Bind<EnemyFactory>().AsSingle();

        Container.Bind<Enemy>().FromInstance(enemyDataBase.EnemyData[EnemyType.Default]).WhenInjectedInto<EnemyFactory>();
    }
}
