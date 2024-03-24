using UnityEngine.Events;

public class LoadingScreenController : ScreenController
{
    public LoadingOperation Operation { get; private set; }
    public UnityAction OnLoadingFinish { get; private set; }

    public LoadingScreenController(LoadingOperation loadingOperation, UnityAction onLoadingFinish)
    {
        OnLoadingFinish = onLoadingFinish;
        Operation = loadingOperation;
    }
}
