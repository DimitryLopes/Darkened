using UnityEngine.Events;

public class GameFinishScreenController : ScreenController
{
    public string GameFinishedText { get; private set; }
    public string GameTypeText { get; private set; }
    public UnityAction OnReplayButtonClicked { get; private set; }
    public UnityAction OnMainMenuButtonClicked { get; private set; }

    public GameFinishScreenController(string gameFinishedText, string gameTypeText,
        UnityAction onPlayButtonClicked, UnityAction onMainMenuButtonClicked)
    {
        OnMainMenuButtonClicked = onMainMenuButtonClicked;
        OnReplayButtonClicked = onPlayButtonClicked;
        GameFinishedText = gameFinishedText;
        GameTypeText = gameTypeText;
    }
}
