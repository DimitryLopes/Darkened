using UnityEngine;
using Zenject;

public class UtilsInstaller : MonoInstaller
{
    [SerializeField]
    private Coroutiner coroutiner;
    [SerializeField]
    private AudioMixingSettings audioSettings;

    public override void InstallBindings()
    {
        Container.Bind<Coroutiner>().FromInstance(coroutiner).AsSingle();
        Container.Bind<AudioMixingSettings>().FromInstance(audioSettings).AsSingle();
    }
}
