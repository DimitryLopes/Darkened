using UnityEngine;
using UnityEngine.Rendering;
using Zenject;

public class UtilsInstaller : MonoInstaller
{
    [SerializeField]
    private Coroutiner coroutiner;
    [SerializeField]
    private AudioMixingSettings audioSettings;
    [SerializeField]
    private Volume postProcessVolume;

    public override void InstallBindings()
    {
        Container.Bind<Coroutiner>().FromInstance(coroutiner).AsSingle();
        Container.Bind<Volume>().FromInstance(postProcessVolume).AsSingle();
        Container.Bind<AudioMixingSettings>().FromInstance(audioSettings).AsSingle();
    }
}
