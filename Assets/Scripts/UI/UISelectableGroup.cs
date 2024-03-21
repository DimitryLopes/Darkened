using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISelectableGroup : MonoBehaviour
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
        items[currentIndex].Select();
    }

    private void Start()
    {
        previousButton.onClick.AddListener(ScrollToPrevious);
        nextButton.onClick.AddListener(ScrollToNext);
    }

    public void ScrollToNext()
    {
        if (currentIndex < items.Count - 1)
        {
            items[currentIndex].Deselect();
            currentIndex++;
            items[currentIndex].Select();

            FinishCurrentAnimation();
            float targetX = selectableItemContainer.anchoredPosition.x - ItemSize;
            currentTween = LeanTween.moveLocalX(selectableItemContainer.gameObject, targetX, 0.5f).setEase(LeanTweenType.easeInOutQuad).
                setOnComplete(FinishCurrentAnimation);
            currentTweenTarget = targetX;
        }
    }

    public void ScrollToPrevious()
    {
        if (currentIndex > 0)
        {
            items[currentIndex].Deselect();
            currentIndex--;
            items[currentIndex].Select();

            FinishCurrentAnimation();
            float targetX = selectableItemContainer.anchoredPosition.x + ItemSize;
            currentTween = LeanTween.moveLocalX(selectableItemContainer.gameObject, targetX, 0.5f).setEase(LeanTweenType.easeInOutQuad).
                setOnComplete(FinishCurrentAnimation);
            currentTweenTarget = targetX;
        }
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
