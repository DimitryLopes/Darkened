using UnityEngine;
using Zenject;

public class DataBaseIntaller : MonoInstaller
{
    [SerializeField]
    private LevelDataBase levelDataBase;
    [SerializeField]
    private ItemDataBase itemDataBase;

    public override void InstallBindings()
    {
        itemDataBase.SetUp();
        Container.Bind<LevelDataBase>().FromInstance(levelDataBase).AsSingle();
        Container.Bind<ItemDataBase>().FromInstance(itemDataBase).AsSingle();
    }
}
