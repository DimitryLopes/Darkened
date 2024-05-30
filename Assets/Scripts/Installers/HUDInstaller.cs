using UnityEngine;
using Zenject;

public class HUDInstaller : MonoInstaller
{
    [SerializeField]
    private HUD hud;
    [SerializeField]
    private Joystick joystick;
    [SerializeField]
    private EnemyArrowPointer enemyArrowPointer;

    public override void InstallBindings()
    {
        Container.Bind<HUD>().FromInstance(hud).AsSingle();
        Container.Bind<Joystick>().FromInstance(joystick).AsSingle();
        Container.Bind<EnemyArrowPointer>().FromInstance(enemyArrowPointer).AsSingle();
    }
}
