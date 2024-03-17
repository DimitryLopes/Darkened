public interface IScreen
{
    bool IsShown { get; }
    void Show<T>(T controller);
    void Hide();
    void OnBeforeShow();
    void OnBeforeHide();
    void OnAfterShow();
    void OnAfterHide();
}