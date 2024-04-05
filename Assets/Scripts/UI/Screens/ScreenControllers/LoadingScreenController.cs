using UnityEngine.Events;

public class LoadingScreenController : ScreenController
{
    public LoadingOperation Operation { get; private set; }
    public UnityAction OnLoadingFinish { get; private set; }
    public AudioManager AudioManager { get; private set; }

    public LoadingScreenController(LoadingOperation loadingOperation, UnityAction onLoadingFinish, AudioManager audioManager)
    {
        OnLoadingFinish = onLoadingFinish;
        Operation = loadingOperation;
        AudioManager = audioManager;
    }
}
