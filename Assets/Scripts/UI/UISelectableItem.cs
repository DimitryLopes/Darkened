using UnityEngine;
using UnityEngine.Events;
using Zenject;
using TMPro;

public class UISelectableItem : MonoBehaviour
{
    [Inject]
    private SignalBus signalBus;

    [SerializeField]
    private Transform itemContainer;
    [SerializeField]
    private UIAnimation animation;
    [SerializeField]
    private TextMeshProUGUI selectableDescription;

    public IUISelectable Selectable { get; private set; }

    public void SetUp(IUISelectable selectable)
    {
        Selectable = selectable;
        selectableDescription.text = selectable.Title;
    }

    public void Select(UnityAction callback = null)
    {
        //animation.DoInAnimation(callback);
        signalBus.Fire(new OnSelectableSelectedSignal(Selectable));
    }

    public void Deselect(UnityAction callback = null)
    {
        //animation.DoOutAnimation(callback);
    }
}
