using System.Collections.Generic;
using UnityEngine;

public class FloatingTextManager
{
    private List<FloatingText> floatingTexts = new();
    private FloatingTextFactory floatingTextFactory;
    private FloatingTextContainer container;

    public FloatingTextManager(FloatingTextFactory factory, EnemyFactory enemyFactory, FloatingTextContainer container)
    {
        floatingTextFactory = factory;
        this.container = container;
    }

    public void ShowFloatingText(string text, Vector3 position)
    {
        FloatingText floatingText = GetAvailableFloatingText();
        Vector3 screenPos = Camera.main.WorldToScreenPoint(position);
        floatingText.transform.position = screenPos;
        floatingText.Show(text);        
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
        newFloatingText.transform.SetParent(container.transform);
        floatingTexts.Add(newFloatingText);
        return newFloatingText;
    }
    #endregion
}
