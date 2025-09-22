using Zenject;
using UnityEngine;

public class UIInstaller : MonoInstaller
{
    [SerializeField]
    private MainCanvas mainCanvas;
    [SerializeField]
    private FloatingTextContainer floatingTextContainer;

    public override void InstallBindings()
    {
        Container.Bind<MainCanvas>().FromInstance(mainCanvas).AsSingle();
        Container.Bind<FloatingTextContainer>().FromInstance(floatingTextContainer).AsSingle();
    }
}
