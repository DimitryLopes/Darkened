using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UILoadingScreen : UIScreen<LoadingScreenController>
{
    private const string LOADING_PROGRESS_TEXT_FORMAT = "{0:0}%";

    [SerializeField]
    private TextMeshProUGUI progressText;
    [SerializeField]
    private TextMeshProUGUI currentTaskText;
    [SerializeField]
    private Image fillProgress;

    private LoadingOperation operation;

    protected override void OnBeforeShow()
    {
        base.OnBeforeShow();
        progressText.text = string.Format(LOADING_PROGRESS_TEXT_FORMAT, 0);
        currentTaskText.text = "Right now I'm currently loading something. If you can read this, this means that somehow the loading went trough the QA testing and the game crashed, please restart it. Sorry for the inconvenience";
        operation = Controller.Operation;
        operation.OnStepChanged += OnStepChanged;
        operation.OnLoadingComplete += Hide;
        Controller.AudioManager.StopBGM();
    }

    protected override void OnAfterShow()
    {
        operation.Load();
    }

    protected override void OnBeforeHide()
    {
        Controller.OnLoadingFinish?.Invoke();
    }

    private void Update()
    {
        progressText.text = string.Format(LOADING_PROGRESS_TEXT_FORMAT, operation.Progress*100);
        fillProgress.fillAmount = operation.Progress;
    }

    private void OnStepChanged(LoadingStep step)
    {
        currentTaskText.text = step.Name;
    }
}
