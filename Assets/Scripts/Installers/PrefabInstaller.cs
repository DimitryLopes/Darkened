using Zenject;
using UnityEngine;
public class PrefabInstaller : MonoInstaller
{
    [SerializeField]
    private Player playerPrefab;

    public override void InstallBindings()
    {
        Container.Bind<Player>().FromInstance(playerPrefab).AsTransient();
    }
}
