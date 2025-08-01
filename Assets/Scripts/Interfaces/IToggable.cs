public interface IToggable
{
    bool Toggled { get; }
    public void Toggle(MazeManager mazeManager);
}
