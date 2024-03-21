public interface IUISelectable
{
    string Title { get; }
    SelectableType Type { get; }
}

public enum SelectableType
{
    MazeSize,
    Difficulty,
    Objective,
    GameMode,
}
