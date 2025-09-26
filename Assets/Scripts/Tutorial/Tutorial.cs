using System;
using UnityEngine;
using Zenject;

public class Tutorial
{
    private TutorialData data;

    public TutorialData Data => data;
    private event Action<Tutorial> onComplete;
    private FloatingText floatingText;

    public Tutorial(TutorialData data, Action<Tutorial> onComplete)
    {
        this.onComplete = onComplete ?? throw new ArgumentNullException(nameof(onComplete));
        this.data = data ?? throw new ArgumentNullException(nameof(data));
    }

    public void Start(FloatingTextManager floatingTextManager, Player player)
    {
        floatingText = floatingTextManager.ShowFloatingText(data.TutorialText, player.transform, -1);
    }

    public void CheckForCompletion()
    {
        foreach (var key in data.RequiredInputs)
        {
            if (Input.GetKeyDown(key))
            {
                floatingText?.InstantHide();
                onComplete?.Invoke(this);
                return; // Exit after the first detected input
            }
        }
    }

}
