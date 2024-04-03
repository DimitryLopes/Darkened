using UnityEngine;
using Zenject;
public class ManagersInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        //Doesn't need other managers
        Container.Bind<HUDManager>().AsSingle();
        Container.Bind<MazeManager>().AsSingle();
        Container.Bind<AudioManager>().AsSingle();
        Container.Bind<EntityManager>().AsSingle();
        Container.Bind<ScreenManager>().AsSingle();
        Container.Bind<ObjectiveManager>().AsSingle();
        //Needs MazeManager
        Container.Bind<LevelManager>().AsSingle();
        //Needs LevelManager
        Container.Bind<GameManager>().AsSingle();

        SceneManager.LoadScene(Constants.Scenes.MAIN_MENU_SCENE);
    }
}
