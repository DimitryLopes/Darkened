using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Zenject;

public class UIGameFinishScreen : UIScreen<GameFinishScreenController>
{
    [SerializeField]
    private TextMeshProUGUI gameFinishedText;
    [SerializeField]
    private TextMeshProUGUI gameTypeText;
    [SerializeField]
    private Button nextLevelButton;
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
        nextLevelButton.onClick.RemoveAllListeners();

        replayButton.onClick.AddListener(Hide);
        replayButton.onClick.AddListener(Controller.OnReplayButtonClicked);

        backToMainMenuButton.onClick.AddListener(Hide);
        backToMainMenuButton.onClick.AddListener(Controller.OnMainMenuButtonClicked);

        nextLevelButton.gameObject.SetActive(Controller.OnNextLevelButtonClicked != null);
        nextLevelButton.onClick.AddListener(Hide);
        nextLevelButton.onClick.AddListener(Controller.OnNextLevelButtonClicked);
    }
}
