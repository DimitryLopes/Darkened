using UnityEngine;
using Zenject;

public class MazeGeneratorInstaller : MonoInstaller
{
    [SerializeField]
    private MazeGenerator mazeGenerator;
    [SerializeField]
    private EntityContainer entityContainer;

    public override void InstallBindings()
    {
        Container.Bind<MazeGenerator>().FromInstance(mazeGenerator).AsSingle();
        Container.Bind<EntityContainer>().FromInstance(entityContainer).AsSingle();
    }
}
