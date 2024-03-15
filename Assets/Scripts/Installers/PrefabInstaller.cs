using Zenject;
using UnityEngine;
public class PrefabInstaller : MonoInstaller
{
    [SerializeField, Header("Player")]
    private Player playerPrefab;
    [SerializeField, Header("Audio")]
    private AudioSource audioSource;

    public override void InstallBindings()
    {
        Container.Bind<Player>().FromInstance(playerPrefab).AsTransient();
        Container.Bind<AudioSource>().FromInstance(audioSource).AsTransient();
    }
}
