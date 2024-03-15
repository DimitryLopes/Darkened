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
    [SerializeField]
    private AudioDataBase audioDataBase;

    public override void InstallBindings()
    {
        itemDataBase.SetUp();
        audioDataBase.SetUp();
        Container.Bind<ItemDataBase>().FromInstance(itemDataBase).AsSingle();
        Container.Bind<AudioDataBase>().FromInstance(audioDataBase).AsSingle();
        Container.Bind<LevelDataBase>().FromInstance(levelDataBase).AsSingle();
        Container.Bind<ObjectivesDataBase>().FromInstance(objectivesDataBase).AsSingle();
    }
}
