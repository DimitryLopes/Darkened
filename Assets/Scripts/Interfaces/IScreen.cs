public interface IScreen
{
    bool IsShown { get; }
    void Show<T>(T controller) where T : ScreenController;
    void Hide();
}