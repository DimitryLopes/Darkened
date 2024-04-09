using UnityEngine;
using UnityEngine.Events;
using Zenject;
using TMPro;

public class UISelectableItem : MonoBehaviour
{
    [Inject]
    private SignalBus signalBus;

    [SerializeField, Header("Small")]
    private GameObject smallContainer;
    [SerializeField]
    private TextMeshProUGUI smallTitle;
    [SerializeField]
    private GameObject bigContainer;
    [SerializeField]
    private TextMeshProUGUI bigTitle;

    public IUISelectable Selectable { get; private set; }

    public void SetUp(IUISelectable selectable, SelectableItemSize size)
    {
        Selectable = selectable;
        SetSize(size);
        switch (size)
        {
            case SelectableItemSize.Small:
                smallTitle.text = selectable.Title;
                return;
            case SelectableItemSize.Big:
                bigTitle.text = selectable.Title;
                return;
        }
    }

    private void SetSize(SelectableItemSize size)
    {
        bigContainer.SetActive(size == SelectableItemSize.Big);
        smallContainer.SetActive(size == SelectableItemSize.Small);
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

public enum SelectableItemSize
{
    Small,
    Big
}
