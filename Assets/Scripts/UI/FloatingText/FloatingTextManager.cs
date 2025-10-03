using System.Collections.Generic;
using UnityEngine;

public class FloatingTextManager
{
    private List<FloatingText> floatingTexts = new();
    private FloatingTextFactory floatingTextFactory;
    private FloatingTextContainer floatingTextContainer;
    private Camera mainCamera;

    public FloatingTextManager(FloatingTextFactory factory, EnemyFactory enemyFactory,
        FloatingTextContainer floatingTextContainer)
    {
        this.floatingTextContainer = floatingTextContainer;
        floatingTextFactory = factory;
        mainCamera = Camera.main;
    }

    public FloatingText ShowFloatingText(string text, Transform position, float duration = Constants.UI.FLOATING_TEXT_DEFAULT_DURATION)
    {
        FloatingText floatingText = GetAvailableFloatingText();

        Vector3 screenPos = Camera.main.WorldToScreenPoint(position.position);
        (floatingText.transform as RectTransform).anchoredPosition = screenPos;

        floatingText.Show(text, mainCamera, duration);
        return floatingText;
    }

    #region Pooling
    private FloatingText GetAvailableFloatingText()
    {
        foreach (var floatingText in floatingTexts)
        {
            if (!floatingText.IsActive)
            {
                return floatingText;
            }
        }

        var newFloatingText = floatingTextFactory.Create();
        floatingTexts.Add(newFloatingText);
        newFloatingText.transform.SetParent(floatingTextContainer.transform);
        return newFloatingText;
    }
    #endregion
}
