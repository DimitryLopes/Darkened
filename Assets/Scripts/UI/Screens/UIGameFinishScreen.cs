using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class UIGameFinishScreen : UIScreen<GameFinishScreenController>
{
    [SerializeField]
    private TextMeshProUGUI gameFinishedText;
    [SerializeField]
    private TextMeshProUGUI gameTypeText;
    [SerializeField]
    private Button replayButton;
    [SerializeField]
    private Button backToMainMenuButton;

    protected override void OnBeforeShow()
    {
        base.OnBeforeShow();
        gameFinishedText.text = Controller.GameFinishedText;

        replayButton.onClick.RemoveAllListeners();
        backToMainMenuButton.onClick.RemoveAllListeners();

        replayButton.onClick.AddListener(Controller.OnReplayButtonClicked);
        replayButton.onClick.AddListener(Hide);
        backToMainMenuButton.onClick.AddListener(Controller.OnMainMenuButtonClicked);
    }
}
