using UnityEngine;
using Zenject;

public class HUDInstaller : MonoInstaller
{
    [SerializeField]
    private HUD hud;

    public override void InstallBindings()
    {
        Container.Bind<HUD>().FromInstance(hud).AsSingle();
    }
}
