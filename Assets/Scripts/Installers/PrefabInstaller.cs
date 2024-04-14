using Zenject;
using UnityEngine;
public class PrefabInstaller : MonoInstaller
{
    [SerializeField, Header("Player")]
    private Player playerPrefab;
    [SerializeField, Header("Audio")]
    private AudioSource audioSource;
    [SerializeField, Header("UI")]
    private MainCanvas mainCanvas;
    [SerializeField]
    private UISelectableItem selectableItem;
    [SerializeField]
    private UIItemView itemView;

    public override void InstallBindings()
    {
        Container.Bind<UISelectableItem>().FromInstance(selectableItem).AsTransient();
        Container.Bind<AudioSource>().FromInstance(audioSource).AsTransient();
        Container.Bind<Player>().FromInstance(playerPrefab).AsTransient();
        Container.Bind<UIItemView>().FromInstance(itemView).AsTransient();
        Container.Bind<MainCanvas>().FromInstance(mainCanvas).AsSingle();
    }
}
