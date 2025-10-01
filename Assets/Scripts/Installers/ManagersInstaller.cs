using Zenject;
public class ManagersInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        //Doesn't need other managers
        Container.Bind<TimeManager>().AsSingle();
        Container.Bind<MazeManager>().AsSingle();
        Container.Bind<EntityManager>().AsSingle();
        Container.Bind<CameraManager>().AsSingle();
        Container.Bind<ScreenManager>().AsSingle();
        Container.Bind<PoolingManager>().AsSingle();
        Container.Bind<MaterialManager>().AsSingle();
        Container.Bind<ObjectiveManager>().AsSingle();
        Container.Bind<PersistenceManager>().AsSingle();
        Container.Bind<FloatingTextManager>().AsSingle();
        Container.Bind<StatusEffectManager>().AsSingle();
        //Needs previous managers
        Container.Bind<HUDManager>().AsSingle();
        Container.Bind<AudioManager>().AsSingle();
        Container.Bind<TutorialManager>().AsSingle();
        Container.Bind<InventoryManager>().AsSingle();
        Container.Bind<UnlockableManager>().AsSingle();
        //Needs previous managers
        Container.Bind<LevelManager>().AsSingle();
        //Needs previous managers
        Container.Bind<GameManager>().AsSingle();

        SceneManager.LoadScene(Constants.Scenes.MAIN_MENU_SCENE);
    }
}
