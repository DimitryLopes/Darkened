using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class DeveloperToolsTrigger : MonoBehaviour
{
    [Inject]
    private DeveloperTools developerTools;
    [Inject]
    private ScreenManager screenManager;

    public float shortPressDuration = 0.3f;
    public float longPressDuration = 1.0f;

    private Button button;
    private enum PressState { None, ShortPress, LongPress, SecondShortPress };
    private PressState currentState = PressState.None;
    private float pressStartTime;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClicked);
    }

    private void Update()
    {
        if (currentState == PressState.None)
        {
            return;
        }

        switch (currentState)
        {
            case PressState.ShortPress:
                if (Time.time - pressStartTime >= shortPressDuration)
                {
                    currentState = PressState.LongPress;
                }
                break;
            case PressState.LongPress:
                if (Time.time - pressStartTime >= longPressDuration)
                {
                    currentState = PressState.SecondShortPress;
                    pressStartTime = Time.time;
                }
                break;
            case PressState.SecondShortPress:
                if (Time.time - pressStartTime >= shortPressDuration)
                {
                    // Trigger event after the complete sequence
                    screenManager.GetScreen<UIDevScreen>().Show(new DevScreenController(developerTools));
                    currentState = PressState.None;
                }
                break;
        }
    }

    private void OnButtonClicked()
    {
        if (currentState == PressState.None)
        {
            currentState = PressState.ShortPress;
            pressStartTime = Time.time;
        }
    }
}
