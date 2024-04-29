using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISelectableGroup : Activateable, IActivateable
{
    private const int ItemSize = 500;

    [SerializeField]
    private Button nextButton;
    [SerializeField]
    private Button previousButton;
    [SerializeField]
    private RectTransform selectableItemContainer;

    private List<UISelectableItem> items;
    private int currentIndex = 0;
    LTDescr currentTween;
    private float currentTweenTarget;

    public RectTransform Container => selectableItemContainer;

    public void Setup(List<UISelectableItem> items)
    {
        this.items = items;
        nextButton.transform.SetAsLastSibling();
    }

    private void Start()
    {
        previousButton.onClick.AddListener(ScrollToPrevious);
        nextButton.onClick.AddListener(ScrollToNext);
    }

    public override void OnActivate()
    {
        if(items == null) return;

        items[currentIndex].Select();
        foreach (UISelectableItem view in items)
        {
            view.Activate();
        }
        ScrollToFirst();
    }

    public override void OnDeactivate()
    {
        foreach(UISelectableItem view in items)
        {
            view.Deactivate();
        }
    }

    public void ScrollTo(int targetIndex)
    {
        if (targetIndex < 0 || targetIndex >= items.Count || targetIndex == currentIndex)
        {
            return;
        }

        float targetX = targetIndex * -ItemSize;

        items[currentIndex].Deselect();
        currentIndex = targetIndex;
        items[currentIndex].Select();

        FinishCurrentAnimation();
        currentTween = LeanTween.moveLocalX(selectableItemContainer.gameObject, targetX, 0.5f).setEase(LeanTweenType.easeInOutQuad).
            setOnComplete(FinishCurrentAnimation);
        currentTweenTarget = targetX;
    }

    public void ScrollToFirst()
    {
        ScrollTo(0);
    }

    public void ScrollToLast()
    {
        ScrollTo(items.Count - 1);
    }

    public void ScrollToNext()
    {
        if (currentIndex >= items.Count - 1)
        {
            ScrollToFirst();
            return;
        }
        ScrollTo(currentIndex + 1);
    }

    public void ScrollToPrevious()
    {
        if (currentIndex == 0)
        {
            ScrollToLast();
            return;
        }
        ScrollTo(currentIndex - 1);

    }

    private void FinishCurrentAnimation()
    {
        if (currentTween != null)
        {
            int id = currentTween.id;
            currentTween = null;
            LeanTween.cancel(id, true);
            Vector3 currentPosition = selectableItemContainer.anchoredPosition;
            selectableItemContainer.anchoredPosition = new Vector3(currentTweenTarget, currentPosition.y, currentPosition.z);
        }
    }


    public void SelectItem(int index)
    {
        items[index].Select();
    }
}
