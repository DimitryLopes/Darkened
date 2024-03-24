using UnityEngine;
using Zenject;

public class UtilsInstaller : MonoInstaller
{
    [SerializeField]
    private Coroutiner coroutiner;

    public override void InstallBindings()
    {
        Container.Bind<Coroutiner>().FromInstance(coroutiner).AsSingle();
    }
}
