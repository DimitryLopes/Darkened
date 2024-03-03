using Zenject;
using UnityEngine;
public class ManagersInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        //MazeManager doesn't need other managers
        Container.Bind<MazeManager>().AsSingle();
        //LevelManager needs MazeManager
        Container.Bind<LevelManager>().AsSingle();
        //GameManager needs LevelManager
        Container.Bind<GameManager>().AsSingle();
    }
}
