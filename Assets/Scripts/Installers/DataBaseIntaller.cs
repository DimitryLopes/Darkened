using UnityEngine;
using Zenject;

public class DataBaseIntaller : MonoInstaller
{
    [SerializeField]
    private LevelDataBase levelDataBase;
    [SerializeField]
    private ItemDataBase itemDataBase;
    [SerializeField]
    private ObjectivesDataBase objectivesDataBase;

    public override void InstallBindings()
    {
        itemDataBase.SetUp();
        Container.Bind<ItemDataBase>().FromInstance(itemDataBase).AsSingle();
        Container.Bind<LevelDataBase>().FromInstance(levelDataBase).AsSingle();
        Container.Bind<ObjectivesDataBase>().FromInstance(objectivesDataBase).AsSingle();
    }
}
