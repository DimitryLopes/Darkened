using UnityEngine.Events;

public class GameFinishScreenController : ScreenController
{
    public string GameFinishedText { get; private set; }
    public UnityAction OnReplayButtonClicked { get; private set; }
    public UnityAction OnNextLevelButtonClicked { get; private set; }
    public UnityAction OnMainMenuButtonClicked { get; private set; }

    public GameFinishScreenController(string gameFinishedText,
        UnityAction onPlayButtonClicked, UnityAction onMainMenuButtonClicked, UnityAction onNextLevelButtonClicked = null)
    {
        OnMainMenuButtonClicked = onMainMenuButtonClicked;
        OnNextLevelButtonClicked = onNextLevelButtonClicked;
        OnReplayButtonClicked = onPlayButtonClicked;
        GameFinishedText = gameFinishedText;
    }
}
