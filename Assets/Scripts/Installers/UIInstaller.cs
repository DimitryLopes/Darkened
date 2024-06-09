using Zenject;
using UnityEngine;

public class UIInstaller : MonoInstaller
{

    [SerializeField]
    private MainCanvas mainCanvas;

    public override void InstallBindings()
    {
        Container.Bind<MainCanvas>().FromInstance(mainCanvas).AsSingle();
    }
}
