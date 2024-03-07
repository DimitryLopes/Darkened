using Zenject;
using UnityEngine;
public class ManagersInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        //Doesn't need other managers
        Container.Bind<MazeManager>().AsSingle();
        Container.Bind<ObjectiveManager>().AsSingle();
        //LevelManager needs MazeManager
        Container.Bind<LevelManager>().AsSingle();
        //GameManager needs LevelManager
        Container.Bind<GameManager>().AsSingle();
    }
}
