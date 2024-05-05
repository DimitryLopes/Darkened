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
    [SerializeField]
    private EntityContainer entityContainer;

    public override void InstallBindings()
    {
        Container.Bind<Coroutiner>().FromInstance(coroutiner).AsSingle();
        Container.Bind<Volume>().FromInstance(postProcessVolume).AsSingle();
        Container.Bind<EntityContainer>().FromInstance(entityContainer).AsSingle();
        Container.Bind<AudioMixingSettings>().FromInstance(audioSettings).AsSingle();
    }
}
