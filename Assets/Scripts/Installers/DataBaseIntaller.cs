using UnityEngine;
using Zenject;

public class DataBaseIntaller : MonoInstaller
{
    [SerializeField]
    private LevelDataBase database;
    public override void InstallBindings()
    {
        Container.Bind<LevelDataBase>().FromInstance(database).AsSingle();
    }
}
