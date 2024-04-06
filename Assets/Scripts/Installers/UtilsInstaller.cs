using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Zenject;

public class UtilsInstaller : MonoInstaller
{
    [SerializeField]
    private Coroutiner coroutiner;
    [SerializeField]
    private AudioMixingSettings audioSettings;
    [SerializeField]
    private PostProcessVolume postProcessVolume;

    public override void InstallBindings()
    {
        Container.Bind<Coroutiner>().FromInstance(coroutiner).AsSingle();
        Container.Bind<AudioMixingSettings>().FromInstance(audioSettings).AsSingle();
        Container.Bind<PostProcessVolume>().FromInstance(postProcessVolume).AsSingle();
    }
}
