using UnityEngine;
using Zenject;

public class MazeGeneratorInstaller : MonoInstaller
{
    [SerializeField]
    private MazeGenerator mazeGenerator;

    public override void InstallBindings()
    {
        Container.Bind<MazeGenerator>().FromInstance(mazeGenerator).AsSingle();
    }
}
