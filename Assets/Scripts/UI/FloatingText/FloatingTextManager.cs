using System.Collections.Generic;
using UnityEngine;

public class FloatingTextManager
{
    private List<FloatingText> floatingTexts = new();
    private FloatingTextFactory floatingTextFactory;
    private Camera mainCamera;
    public FloatingTextManager(FloatingTextFactory factory, EnemyFactory enemyFactory)
    {
        floatingTextFactory = factory;
        mainCamera = Camera.main;
    }

    public FloatingText ShowFloatingText(string text, Transform parent, float duration = Constants.UI.FLOATING_TEXT_DEFAULT_DURATION)
    {
        FloatingText floatingText = GetAvailableFloatingText();
        Vector3 screenPos = Camera.main.WorldToScreenPoint(parent.position);
        floatingText.transform.SetParent(parent.transform);
        floatingText.transform.position = parent.position;
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
        return newFloatingText;
    }
    #endregion
}
