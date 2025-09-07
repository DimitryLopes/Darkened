using Zenject;
using UnityEngine;
public class PrefabInstaller : MonoInstaller
{
    [SerializeField, Header("Player")]
    private Player playerPrefab;
    [SerializeField, Header("Audio")]
    private AudioSource audioSource;
    [SerializeField, Header("UI")]
    private UISelectableItem selectableItem;
    [SerializeField]
    private UIItemView itemView;
    [SerializeField]
    private FloatingText floatingText;
    [SerializeField]
    private VisualEntityStatusEffect visualEntityStatusEffect;

    public override void InstallBindings()
    {
        Container.Bind<Player>().FromInstance(playerPrefab).AsTransient();
        Container.Bind<UIItemView>().FromInstance(itemView).AsTransient();
        Container.Bind<AudioSource>().FromInstance(audioSource).AsTransient();
        Container.Bind<FloatingText>().FromInstance(floatingText).AsTransient();
        Container.Bind<UISelectableItem>().FromInstance(selectableItem).AsTransient();
        Container.Bind<VisualEntityStatusEffect>().FromInstance(visualEntityStatusEffect).AsTransient();
    }
}
