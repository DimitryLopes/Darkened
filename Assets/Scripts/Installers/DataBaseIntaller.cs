using UnityEngine;
using Zenject;

public class DataBaseIntaller : MonoInstaller
{
    [SerializeField]
    private UIScreenDataBase screenDataBase;
    [SerializeField]
    private LevelDataBase levelDataBase;
    [SerializeField]
    private ItemDataBase itemDataBase;
    [SerializeField]
    private ObjectivesDataBase objectivesDataBase;
    [SerializeField]
    private AudioDataBase audioDataBase;
    [SerializeField]
    private MazeSizeDataBase mazeSizeDataBase;
    [SerializeField]
    private EnemyDataBase enemyDataBase;
    [SerializeField]
    private DifficultyDataBase difficultyDataBase;
    [SerializeField]
    private MaterialDataBase materialDataBase;

    public override void InstallBindings()
    {
        itemDataBase.SetUp();
        levelDataBase.SetUp();
        enemyDataBase.SetUp();
        audioDataBase.SetUp();
        screenDataBase.SetUp();
        materialDataBase.SetUp();
        objectivesDataBase.SetUp();
        difficultyDataBase.SetUp();

        Container.Bind<ItemDataBase>().FromInstance(itemDataBase).AsSingle();
        Container.Bind<LevelDataBase>().FromInstance(levelDataBase).AsSingle();
        Container.Bind<AudioDataBase>().FromInstance(audioDataBase).AsSingle();
        Container.Bind<EnemyDataBase>().FromInstance(enemyDataBase).AsSingle();
        Container.Bind<UIScreenDataBase>().FromInstance(screenDataBase).AsSingle();
        Container.Bind<MaterialDataBase>().FromInstance(materialDataBase).AsSingle();
        Container.Bind<ObjectivesDataBase>().FromInstance(objectivesDataBase).AsSingle();
        Container.Bind<DifficultyDataBase>().FromInstance(difficultyDataBase).AsSingle();

        Container.Bind<MazeSizeDataBase>().FromInstance(mazeSizeDataBase).AsSingle();
    }
}
